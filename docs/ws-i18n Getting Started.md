## Getting started with Web Services Internationalization

Full specifications can be found [here](http://www.w3.org/TR/ws-i18n/)

WS-I18N is a standardized extension to SOAP messaging intended to allow internationalized and localized operations using locale and international preferences communicated by consumers to providers of SOAP XML services. Alone, WS-I18N as a standard does not ensure or require globalization support. The standard was drafted to support a common messaging component that can be used in conjunction with other Web services and application-specific APIs to support globalization. It does not dictate the sceanrio usage or patterns that MUST be implemented to support the specification but does document and suggest basic guidelines for use. It was intended to provide a message based (thus supporting end to end) vs transport based support (point to point and HTTP centric only) of globalization preferences.

WS-I18N is intended to viewed for applicability in a solution via the following 4 scenarios:
# Locale Neutral. Most aspects of most services are not particularly locale-affected. These services can be considered "locale neutral". For example, a service that adds two integers together is locale neutral.
# Data Driven. Aspects of the data determine how it is processed, rather than the configuration of either the requester or the provider.
# Service Determined. The service will have a particular setting built into it. As in: this service always runs in the French for France locale. Or, quite commonly, the service will run in the host's default locale. It may even be a deployment decision that controls which locale or preferences are applied to the service's operation.
# Client Influenced. The service's operation can use a locale preference provided by the end-user to affect its processing. This is called "influenced" because not every request may be honored by the service (the service may only implement behavior for certain locales or international preference combinations).

While not a technical restriction, the Common Contracts Globalization contract (hereafter Contract or Library) was designed to primarily support scenarios #3 and 4 above. It can be leveraged as part of a larger protocol or scenario as required by your implementation. It should be noted that any of these patterns may apply to a service as a whole or an aspect of the service (e.g. portType Operation specifically).

_technical note_: _This Library does not support the facility to indicate use of WS-I18N via WS-Policy Attachment (see [section 4](http://www.w3.org/TR/ws-i18n/#sec-ws-i18n-policy)) in a SOAP WSDL document. The Library limits scope to the runtime/message exchange level only. Therefore any WS-I18N information emited will be limited to explicit declaration of message members ONLY._

### Structure of WS-I18N Message Components

The specification codifies the ability to:
# indicate the locale and/or language preference of the client to the Web service and service provider
# indicate the time zone of the client
# indicate additional optional information about the client's international preferences
# provide an extensible mechanism for adding other related information to the request


This specification uses a number of namespace prefixes throughout; they are listed in Table 1. Note that the choice of any namespace prefix is arbitrary and not semantically significant.

|| Prefix || Namespace || Specification ||
| i18n | http://www.w3.org/2005/09/ws-i18n | Internationalization Data Structures for SOAP Messages |
| ldml | http://unicode.org/cldr/ | [LDML](http://unicode.org/reports/tr35/tr35-8.html) |
| S | http://www.w3.org/2003/05/soap-envelope | [SOAP 1.2](http://www.w3.org/TR/soap12-part1/) |
| xs | http://www.w3.org/2001/XMLSchema | XML Schema |

All interaction is intended to occur via the i18n:international element. The normative form is:
{{
<i18n:international S:actor="...">
 <i18n:locale> locale identifier </i18n:locale>
 <i18n:timezone> time zone value </i18n:timezone>
 <i18n:preferences ...>
  LDML-based or other locale preferences
 </i18n:preferences>
</i18n:international>}}

### Indicating Service Determined Culture/Locale Information

The easiest scenario will be to enable a service to indicate the leveraged or used locale information as determined by the service implementation. Normally this would be leveraged to provide globalization context textual content or other human readable content. In this pattern, the Library would be leveraged in one of two ways:

#### declarative contract
In this form the presence of the i18n:international element would be explicitly declared as a member of the SOAP messages (in this case the output message) typcially communicated via a SOAP message header (though not required). To implement this pattern:

explicitly declare your message contracts
{{
[MessageContract()](MessageContract())
public class MyRequestContract
{ /**omitted for brevity**/ }

[MessageContract()](MessageContract())
public class MyResponseContract
{
  [MessageHeader()](MessageHeader())
  public International International
  { get; set;
  }
} }}

create your service contract
{{
[ServiceContract()](ServiceContract())
public interface IMyService
{
  [OperationContract()](OperationContract())
  MyResponseContract MyOperation(MyRequestContract request);
} }}

implement with the default behavior
{{
public class MyService : IMyService
{
  public MyResponseContract MyOperation(MyRequestContract request)
  {
    // do work
    response.International = new International(Thread.CurrentThread.CurrentCulture); //use the current thread culture
    return response;
  }
} }}

#### SOAP Header Extensibility
In this form the presence of the i18n:international element is not explicitly declared as a member of the SOAP WSDL document. The service implementation will typcially add a SOAP message header to indicate this information. Usually this is done when use of WS-I18N is communicated / documented out of band and is known to client and service or when implementing an existing contract and extending support for web services internationalization. To implement this pattern:

create your service contract
_note: the use of explicit message contracts is not required in this approach._
{{
[ServiceContract()](ServiceContract())
public interface IMyService
{
  [OperationContract()](OperationContract())
  int MyOperation(String request);
} }}


create an IDispatchMessageInspector implementation
{{
public class InternationalizationMessageInspector : IDispatchMessageInspector
{
  public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
  {
    **/No Op **/
    return null;
  }

  public void BeforeSendReply(ref Message reply, object correlationState)
  {
    var international = new International(Thread.CurrentThread.CurrentCulture);
    var header = new MessageHeader<International>(international);
    reply.Headers.Add(header.GetUntypedHeader("international2", Constants.Namespace));
  }
} }}

Application of the inspector is usually accomplished with a service behavior. See [MSDN](http://msdn.microsoft.com/en-us/library/system.servicemodel.dispatcher.idispatchmessageinspector.aspx) for examples of how to create an IServiceBehavior to apply your inspector.

### Indicating Client Culture Preferences

For service clients that need to send culture information to services that are not part of the published contract (an implementation, not contract feature) the Library supports a convenience component to include header information on each outgoing request. To enable outgoing automatic culture information of service clients:

generate client service proxy
{{
//Example generated service interface
[ServiceContract()](ServiceContract())
public interface ISomeService
{
  [OperationContract()](OperationContract())
  int SomeOperation(string request);

  [OperationContract()](OperationContract())
  void AnotherOperation();
} }}

enable culture flow on each desired operation
{{
//Example generated service interface
[ServiceContract()](ServiceContract())
public interface ISomeService
{
  //Enable automatic culture from the thread to flow
  [GlobalizationClientAttribute(true)](GlobalizationClientAttribute(true))
  [OperationContract()](OperationContract())
  int SomeOperation(string request);

  //Enable explicitly indicated culture to be sent instead
  [GlobalizationClientAttribute(false, CultureInfo = "EN-us")](GlobalizationClientAttribute(false,-CultureInfo-=-_EN-us_))
  [OperationContract()](OperationContract())
  void AnotherOperation();
} }}

If editing of the generated client proxy contract is not an option, the direct use of the GlobalizationClientMessageInspector class can be used to accomplish the same goal. See [MSDN](http://msdn.microsoft.com/en-us/library/system.servicemodel.dispatcher.iclientmessageinspector.aspx) for documentation on how to configure message inspectors.