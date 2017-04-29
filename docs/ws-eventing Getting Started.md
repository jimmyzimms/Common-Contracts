Full specifications can be found [here](http://www.w3.org/Submission/WS-Eventing)

Web services often want to receive messages when events occur in other services and applications. The WS-Eventing specification provides a mechanism for registering interest in receiving such messages from a service or application. This specification defines a protocol for one Web service (called a "subscriber") to register interest (called a "subscription") with another Web service (called an "event source") in receiving messages about events (called "notifications" or "event messages"). The subscriber may manage the subscription by interacting with a Web service (called the "subscription manager") designated by the event source. A subscription may have a designated expiration that can be requested by a subscriber and granted or dictated by an event source. The subscription manager provides the ability for the subscriber to renew or cancel the subscription before it expires.
 
There are many mechanisms by which event sources may deliver events to event sinks. The specification provides an extensible way for subscribers to identify the delivery mechanism they prefer. While asynchronous, pushed delivery is defined specifically in the specification, the intent is that there should be no limitation or restriction on the delivery mechanisms capable of being supported by the specification. This library does not restrict an implementation in working with any specific infrastructure or methodology.

Structure of WS-Eventing Message Components
The specification codifies the ability to: 
# Create and delete event subscriptions.
# Define expiration for subscriptions and allow them to be renewed.
# Define how one Web service can subscribe on behalf of another.
# Define how an event source delegates subscription management to another Web service.
# Allow subscribers to specify how event messages should be delivered.

This specification uses a number of namespace prefixes throughout; they are listed in Table 1. Note that the choice of any namespace prefix is arbitrary and not semantically significant.

|| Prefix || Namespace || Specification ||
| s | (Either SOAP 1.1 or 1.2) | (Either SOAP 1.1 or 1.2) |
| s11 | http://schemas.xmlsoap.org/soap/envelope/ | [SOAP 1.1](http://www.w3.org/TR/2000/NOTE-SOAP-20000508/) |
| s12 | http://www.w3.org/2003/05/soap-envelope | [SOAP 1.2](http://www.w3.org/TR/soap12-part1/) |
| wsdl | http://schemas.xmlsoap.org/wsdl/ | [WSDL](http://www.w3.org/TR/2001/NOTE-wsdl-20010315) |
| wsa | http://schemas.xmlsoap.org/ws/2004/08/addressing | [WS-Addressing](http://www.w3.org/Submission/2004/SUBM-ws-addressing-20040810/) |
| wse | http://schemas.xmlsoap.org/ws/2004/08/eventing | [WS-Eventing](http://www.w3.org/Submission/WS-Eventing/) |
| xs | http://www.w3.org/2001/XMLSchema | XML Schema |

## Subscriptions
The first use case in WS-Eventing is for a system interested in being notified regarding events from a source to register interest. This is accomplished via invoking the Subscribe operation on the event source.

To create a subscription, a subscriber sends a request message of the following form to an event source:
{{
<wse:Subscribe …>
 <wse:EndTo>endpoint-reference</wse:EndTo> ?
 <wse:Delivery Mode="xs:anyURI"? >xs:any</wse:Delivery>
 <wse:Expires>[xs:dateTime ](-xs_duration)</wse:Expires> ?
 <wse:Filter Dialect="xs:anyURI"? > xs:any </wse:Filter> ?
 …
</wse:Subscribe>
}}

In the library this is handled by a service implementing the IEventSource interface.

**note: in this document examples will be usually be illustrated in such a way that host, binding, security and where responsibilities are implemented are not considered when discussing WS-Eventing constructs. This is shown this way for brevity. For more detail on various implementation considerations, please see the [Scenarios](ws-eventing-Scenarios) document. In addition, various uses of WCF are shown without regard to best practices in service implementation, again for brevity.**

{{
public class Service : IEventSource
{
  public SubscribeResponseMessage Subscribe(SubscribeRequestMessage request)
  {
    //omitted for brevity
  }
}
}}

The SubscribeRequestMessage corresponds to the abstract concept of the request message itself. It does not have a concrete representation in the received message content. The request message contains a single pre-defined message header that is custom to the library. Please see [Scenarios](ws-eventing-Scenarios) document for using this proprietary element. A single SubscribeRequestMessageBody item will be contained in the message. Each child element in the wse:Subscribe element is represented as a property on the class.

How to get the address of the end subscription EPA:
{{
EndpointAddress sendEndNotificationsTo = request.EndTo.ToEndpointAddress();
Uri address = sendEndNotificationsTo.Uri;
}}

Property: EndTo
Type: EndpointAddressAugust2004
Relates To: /s:Envelope/s:Body/*/wse:EndTo

Describes where to send a SubscriptionEnd message if the subscription is terminated unexpectedly. If present, this element MUST be of type wsa:EndpointReferenceType in the request message. This value is not required and may be null. By default the behavior is to not send this element.

How to understand how and where to deliver the event messages:
{{
EndpointAddress notifyTo = request.Delivery.NotifyTo.ToEndpointAddress();
Uri address = notifyTo.Uri;
Uri mode = request.Delivery.Mode;
}}

Property: Delivery
Type: Delivery
Relates To: /s:Envelope/s:Body/*/wse:Delivery 

Contains the delivery destination for notification messages to be sent to for the subscription and an indicator for how the notification messages sent in relation to this subscription should be handled. The implied value is a Uri value of "http://schemas.xmlsoap.org/ws/2004/08/eventing/DeliveryModes/Push", which indicates that Push Mode delivery should be used. See Delivery Modes for more information. 

If the event source does not support the requested delivery mode, the request MUST fail, and the event source MAY generate a wse:DeliveryModeRequestedUnavailable fault indicating that the requested delivery mode is not supported. This can be modeled using the SupportedDeliveryModeFault type in the library.

{{
var fault = new SupportedDeliveryModeFault(supportedModes);
throw new FaultException<SupportedDeliveryModeFault>(fault);
}}

How to understand expiration dates:
{{
var expiration = request.Expiration;
if (expiration == null)
{
  // infinite subscription requested
}
else
{
  DateTime expDate = expiration.Value;

  // dates will always create a UTC based date and will coerce timespan
  // values based on the current machine clock
  Debug.Assert(expDate.Kind == DateTimeKind.Utc);
}
}}

Property: Expires
Type: Expires
Relates To: /s:Envelope/s:Body/*/wse:Expires 

Contains the requested expiration time for the subscription. (No implied value.) The event source defines the actual expiration and is not constrained to use a time less or greater than the requested expiration. The expiration time may be a specific time or a duration from the subscription's creation time. Both specific times and durations are interpreted based on the event source's clock.
 
If this element does not appear, then the request is for a subscription that will not expire. That is, the subscriber is requesting the event source to create a subscription with an indefinite lifetime. If the event source grants such a subscription, it may be terminated by the subscriber using an Unsubscribe request, or it may be terminated by the event source at any time for reasons such as connection termination, resource constraints, or system shut-down.
 
If the expiration time is either a zero duration or a specific time that occurs in the past according to the event source, then the request MUST fail, and the event source MAY generate a wse:InvalidExpirationTime fault indicating that an invalid expiration time was requested.

{{
var fault = new InvalidExpirationTimeFault();
throw new FaultException<InvalidExpirationTimeFault>(fault);
}}

Some event sources may not have a "wall time" clock available, and so are only able to accept durations as expirations. If such a source receives a Subscribe request containing a specific time expiration, then the request MAY fail; if so, the event source MAY generate a wse:UnsupportedExpirationType fault indicating that an unsupported expiration type was requested.

Determining Filters and Dialects:
{{

  var isXPath = request.FilterDialect == Constants.WsEventing.Dialects.XPath;
  if (!isXPath)
  {
    // Currently only XPath filter statements are supported
    Debug.Assert(request.Filter == null);
  }
  else
  {
    XPathMessageFilter filter = request.Filter;
    if (filter == null)
    {
      // There is no filter e.g. All events should be delivered regardless of content
    }
    else
    {
      String xpathValue = filter.XPath;
    }
}
}}

Property: FilterDialect
Type: String
Relates To: /s:Envelope/s:Body/*/wse:Filter/@Dialect 
Implied value is "http://www.w3.org/TR/1999/REC-xpath-19991116".
 
While an XPath predicate expression provides great flexibility and power, alternate filter dialects may be defined. For instance, a simpler, less powerful dialect might be defined for resource-constrained implementations, or a new dialect might be defined to support filtering based on data not included in the notification message itself. If desired, a filter dialect could allow the definition of a composite filter that contained multiple filters from other dialects. This library does not support dialects other than XPath at this time (though does not constrain the value in the request message).

Property: Filter
Type: XPathMessageFilter
Relates To: /s:Envelope/s:Body/*/wse:Filter 

A Boolean expression in XPath. If the expression evaluates to false for a notification, the notification MUST NOT be sent to the event sink. Implied value is an expression that always returns true. If the event source does not support filtering, then a request that specifies a filter MUST fail, and the event source MAY generate a wse:FilteringNotSupported fault indicating that filtering is not supported.

{{
var fault = new FilteringNotSupportedFault();
throw new FaultException<FilteringNotSupportedFault>(fault);
}}

Currently the only filter language supported is XPath and if the correct dialect is supported, the expression will be extracted into an XPathMessageFilter instance. If the dialect is not XPath, the dialect value will be set and the filter will be ignored.

If the event source supports filtering but cannot honor the requested filtering, the request MUST fail, and the event source MAY generate a wse:FilteringRequestedUnavailable fault indicating that the requested filter dialect is not supported.

{{
var fault = new SupportedDialectFault(supportedDialects);
throw new FaultException<SupportedDialectFault>(fault);
}}

If the event source chooses not to accept a subscription, the request MUST fail, and the event source MAY generate a wse:EventSourceUnableToProcess fault indicating that the request was not accepted.

{{
var fault = new EventSourceUnableToProcessFault();
throw new FaultException<EventSourceUnableToProcessFault>(fault);
}}

If the event source accepts a request to create a subscription, it MUST reply with a response of the following form:
{{
<wse:SubscribeResponse …>
 <wse:SubscriptionManager>
  wsa:EndpointReferenceType
 </wse:SubscriptionManager>
 <wse:Expires>[xs:dateTime ](-xs_duration)</wse:Expires>
 …
</wse:SubscribeResponse>
}}

When a subscription is accepted the service will generate a SubscribeResponseMessage containing the information describing the subscription and provide a location for the subscriber to manage this subscription.

Determine an expiration date:
An event source has final say regarding any event subscription duration. It may or may not take into consideration any requested expiration when determining this value. It is completely internal logical to the service.

Property: Expiration
Type: Expiration
Relates To: /s:Envelope/s:Body/*/wse:Expires 
The expiration time assigned by the event source. The expiration time MAY be either an absolute time or a duration but SHOULD be of the same type as the requested expiration (if any).
 
If this element does not appear, then the subscription will not expire. That is, the subscription has an indefinite lifetime. It may be terminated by the subscriber using an Unsubscribe request, or it may be terminated by the event source at any time for reasons such as connection termination, resource constraints, or system shut-down.

{{
//Indicates an infinite subscription
Expiration expiration == null;

//Indicates a subscription with a specific end time
var expDate = DateTime.Now;
expiration = new Expiration(expDate);
}}

It is important to note that UTC and system local times are supported, however a local time will always be coerced into UTC. Indicating values via timespan is not supported.
{{
Debug.Assert(expDate.Kind != DateTimeKind.Utc);
Debug.Assert(expiration.Value.Kind == DateTimeKind.Utc);
}}

Indicate the location of the subscription manager used by the subscriber to managae this subscription instance:
{{
var someAddress = new Uri("http://someplace");
var manager = new SubscriptionManager(someAddress);
}}

Property: SubscriptionManager
Type: SubscriptionManager
Relates To: /s:Envelope/s:Body/*/wse:SubscriptionManager 

In some cases, it is convenient for all EPRs issued by a single event source to address a single Web service (or itself) and be able to use a reference parameter to distinguish among the active subscriptions. For convenience in this common situation, this specification defines a global element, wse:Identifier of type xs:anyURI, that MAY be used as a distinguishing reference parameter if desired by the event source. It is common to supply this either in the SubscriptionManager EPR returned or as part of the response headers.

While both are supported it is important to note they MUST be the same value.

{{
// By default the identifier type will use a dymanically created Guid value
var identifier = new Identifier();

var someAddress = new Uri("http://someplace");
var manager = new SubscriptionManager(someAddress, identifier);
}}

Finally create the response wrapper:
{{
var response = new SubscribeResponseMessage(new SubscribeResponseMessageBody(expires, manager));

//Optionally the identifier can be applied as a response header
response.Identifier = identifier;
}}

## Managing Subscriptions

Once a subscription is created, all management occurs via the SubscritpionManager interface. The event source and subscription manager may or may not be accessable via the same endpoint. It is not uncommon for event sources to rely on a single subscription manager for managing subscribers as part of the application or even external 3rd party services. This library does not prevent any approach and the individual interfaces are seperately modeled and implemented independently by a service.

### Renewing Subscriptions

To update the expiration for a subscription, a subscriber will send requests for an existing subscriptions to the subscription manager. It is important to note that only updates to the subscription expiration are supported. Changing other subscription information, such as NotifyTo/EndTo endpoints or filters, is not supported. Please see the Using WS-Eventing section for recomendations.

To renew a subscription, the subscriber sends a request of the following form to the subscription manager:
{{
<wse:Renew …>
 <wse:Expires>[xs:dateTime ](-xs_duration)</wse:Expires> ?
 …
</wse:Renew>
}}
 
If the subscription manager accepts a request to renew a subscription, it MUST reply with a response of the following form:
{{
<wse:RenewResponse …>
 <wse:Expires>[xs:dateTime ](-xs_duration)</wse:Expires> ?
 …
</wse:RenewResponse>
}}
 
/s:Envelope/s:Body/*/wse:Expires 
If the requested expiration is a duration, then the implied start of that duration is the time when the subscription manager starts processing the Renew request.
 
If the subscription manager chooses not to renew this subscription, the request MUST fail, and the subscription manager MAY generate a wse:UnableToRenew fault indicating that the renewal was not accepted.
 
The specification does not specifically outline the behavior for requests for subscriptions that it is unable to identify. It is implied this condition will use the wse:UnableToRenew fault with a specific message explaining the reason.

## GetStatus
To get the status of a subscription, the subscriber sends a request of the following form to the subscription manager:
{{
<wse:GetStatus …>
 …
</wse:GetStatus>
}}

If the subscription is valid and has not expired, the subscription manager MUST reply with a response of the following form:
{{
<wse:GetStatusResponse …>
 <wse:Expires>[xs:dateTime ](-xs_duration)</wse:Expires> ?
 …
</wse:GetStatusResponse>
}}

