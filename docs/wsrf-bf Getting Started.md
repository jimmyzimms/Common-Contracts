Full specifications can be found [here](http://docs.oasis-open.org/wsrf/wsrf-ws_base_faults-1.2-spec-os.pdf)

A designer of a Web services application often uses interfaces defined by others. Managing faults in such an application is more difficult when each interface uses a different convention for representing common information in fault messages. Support for problem determination and fault management can be enhanced by specifying Web services fault messages in a common way. When the information available in faults from various interfaces is consistent, it is easier for requestors to understand faults. It is also more likely that common tooling can be created to assist in the handling of faults. WS-BaseFaults defines an XML Schema type for base faults, along with rules for how this base fault type is used and extended by Web services. Web services adoption is facilidated and simplified by standardizing a base set of information that may appear in fault messages. (-taken from the OASIS specification)

Structure of WSRF-Base Faults Message Components
The specification codifies the ability to: 
# Define a standard XML Schema type containing the minimum base fault information
# Define a methodology for chaining fault information together in a fashion to enable root fault cause reporting

This specification uses a number of namespace prefixes throughout; they are listed in Table 1. Note that the choice of any namespace prefix is arbitrary and not semantically significant.

|| Prefix || Namespace || Specification ||
| wsrf-bf | http://docs.oasis-open.org/wsrf/bf-2 | WS-Base Faults |
| S | http://www.w3.org/2003/05/soap-envelope | [SOAP 1.2](http://www.w3.org/TR/soap12-part1/) |
| xs | http://www.w3.org/2001/XMLSchema | XML Schema |
| wsa | http://www.w3.org/2005/08/addressing | [WS-Addressing](http://www.w3.org/TR/2006/REC-ws-addr-core-20060509/) |
| xsi | http://www.w3.org/2001/XMLSchema-instance | XML Schema Instance |

All interaction is intended to occur via the S:fault/S:detail element. The normative form of the detail inner content is:
{{
<BaseFault>
 <Timestamp>xsd:dateTime</Timestamp>
 <OriginatorReference>
  wsa:EndpointReferenceType
 </OriginatorReference> ?
 <ErrorCode dialect="anyURI">xsd:anyType</ErrorCode> ?
 <Description>xsd:string</Description> *
 <FaultCause>{any}</FaultCause> ?
</BaseFault>}}

### The Basic Use Case

The most common scenario for creating WS-Base Faults compliant services is to provide the basic information required for protocol compliance. This is accomplished via XSD extension of the wsrf-bf:BaseFaultType with a new specific schema type that provides no additional information/structure elements that can be leveraged in your services. This can be accomplished declaring a new Fault Contract type, subclassing the library BaseFault abstract class, and adding a new distinct DataContractAttribute to your new contract for the new fault.

{{
[DataContract(Name = "MyNewFault", Namespace = "urn:MyNamespace")](DataContract(Name-=-_MyNewFault_,-Namespace-=-_urn_MyNamespace_))
public class MyNewFault : CommonContracts.BaseFault
{
}
}}

This then can be declared on your service contracts via the standard fault contract mechanism:
{{
public interface IMyService
{
  [FaultContract(typeof(MyNewFault))
  void MyOperation();
}
}}

To use at runtime, regardless of WSDL declaration, use the FaultException<T> class to return the fault information to the caller:
{{
public class MyService : IMyService
{
  public void MyOperation()
  {
    throw new FaultException<MyNewFault>(new MyNewFault());
  }
}
}}

This would serialize as the following XML to be sent to the client:
{{
<Fault xmlns="http://www.w3.org/2003/05/soap-envelope" xmlns:S12="xmlns="http://www.w3.org/2003/05/soap-envelope">
 <Code>
  <Value>s12:Sender</Value>
 </Code>
 <Reason>
  <Text xml:lang="en">Custom Fault</Text>
 </Reason>
 <Detail>
  <MyNewFault xmlns="urn:mynamespace">
   <Timestamp xmlns="http://docs.oasis-open.org/wsrf/bf-2">
    2005-05-04T20:18:44.970Z
   </Timestamp>
  </MyNewFault>
 <Detail>
</Fault>
}}

#### Adding Additional Information

In order to add any additional information in the fault message being returned, follow the previously outlined steps and enlist any additional structural information required via the standard Data Contract methology:
{{
[DataContract()](DataContract())
public class MyExtendedFault : MyNewFault
{
  [DataMember(Order = 2)](DataMember(Order-=-2))
  public String AdditionalInformation { get; set; }
}
}}

**important**: To make sure that serialization and versioning issues do not arrise with use of your contracts, attempt to always use an Order value greater than 1 with your additional contract elements. For more information see [MSDN](http://msdn.microsoft.com/en-us/library/ms733832.aspx).

### Advanced Fault Details

The wsrf-bf specification supports optional information and capabilities for detail for advanced use cases that expose additional reporting fidelity . Additional information in the core specification include:

* OriginatorReference
An OPTIONAL element is a WS-Addressing [WS-Addressing](WS-Addressing) EndpointReference of the Web service that generated the fault. This element MAY be omitted if the fault originator is clearly implied by the context in which the fault appears (for example in a simple request response message exchange). One use of this element is in a situation of nested or chained faults. Another common pattern is to support indicating the fault cause for composite service scenarios (Service A is a composite of Service B and C, one of which was the root fault location that gave direct rise to this fault) regardless of fault chaining details. See the FaultCause element below.

* ErrorCode
An OPTIONAL element provides convenient support for legacy fault reporting systems (e.g., POSIX errno, HRESULT, or other response code). The dialect attribute on ErrorCode MUST be a URI that defines the context in  which the ErrorCode MUST be interpreted. For example, a URI might be defined that describes how a POSIX errno is mapped to a ErrorCode and that URI must appear on any ErrorCode element carrying a POSIX errno.

* Description
An OPTIONAL element contains a plain language description of the fault. This description is expected to be helpful in explaining the fault to users. There MAY be any number of description elements.

* FaultCause
An OPTIONAL element, if present, MUST contain a wsrf-bf BaseFault element or an element whose type extends the BaseFaultType that describes an underlying cause of this fault. The ability to include a FaultCause element in a fault allows for chaining of fault information so that a recipient of a fault MAY examine details underlying the cause of the fault. Note that there is no required child element within BaseFault that identifies the particular type (or class) of fault. Rather, an application-specific extension of BaseFault MUST be defined for each distinct type of fault. This structure is similar to how structured exception handling works in .Net or Java. The use of this element should be limited and restricted to diagnostics only in order to not create dependencies between end clients and secondary internal systems in composite services.

Each of the above elements can be used independently of each other and each facilitates the need to leverage the XmlSerializer capabilities of WCF. In order to leverage this functionality use the BaseFaultFull type in the library instead. The following code shows how to add additional information to the fault:

{{
// assumes you have defined a new custom fault type previously
var fault = new MyCustomFault();

// Now add an Originator element to it. In this example we are going to
// simply add the current service call EPA to the fault. In a typical Request-
// Reply scenario this is redundant.
var epa = OperationContext.Current.Channel.LocalAddress;
fault.Originator = EndpointAddress10.FromEndpointAddress(epa);

// Add a single description. This example will leverage the current
// thread culture. There is an overload allowing it to be specified.
fault.Descriptions.Add(new Description("a description"));

// Add a default ErrorCode structure. This basic form only contains an @dialect attribute
var dialect = new Uri("http://myerror");
fault.ErrorCode = new ErrorCode(dialect);

// Add a strongly typed mixed ErrorCode structure. This one will contain a child element previously defined.
fault.ErrorCode = new ErrorCode<MyErrorInfo>(dialect, new MyErrorInfo());
}}

## Creating a Custom Xml Serializer Fault

Creating new faults based on the BaseFaultFull type is simple and easy. Simply follow the steps outlined in this section for each custom fault type you create. Each step will need to be taken to fully support the various options exposed by the library, however, if a particular feature is not required then it can be skipped (for example, a fault that is only ever used by a service to return information to a caller can skip the deserialization support).

### Writing Fault Information to XML

To enable a custom fault to be serialized perform the following steps:

# Create your new custom fault type and subclass the BaseFaultFull class.
# Add the XmlRoot attribute to the class declaration. Supply the local name and namespace of the root element for the fault detail.
# Override the WriteStartElement and WriteEndElement methods. If no additional information needs to be written (such as attributes or additional child elements) then make this method a no-op implementation.
# If any additional fault content is required to be serialized, override the ProcessAdditionalElements method. Add any additional XmlWriter operations required to write the additional information. Normally this would be used to serialize additional fault properties specific to your custom fault.
# OPTIONAL: If your fault will be exposed via the SOAP WSDL (declared via the FaultContract attribute) then support XML Schema by creating a new public static method to return the schema information. The method should accept an XmlSchemaSet parameter and return either an XmlQualifiedName or XmlSchemaType (for anonymous XML types). Add an XmlSchemaProvider attribute to your class declaration indicating your schema method. More information on using the XmlSchemaProvider attribute can be read [here](http://msdn.microsoft.com/en-us/library/system.xml.serialization.xmlschemaproviderattribute.aspx)
# Add any required constructors to support the expect use cases. The most common, though not required, constructor overloads are
{{
public YourFault() { }
public YourFault(EndpointAddress originator) : base(originator) {}
public YourFault(IEnumerable<Description> descriptions) : base(descriptions) {}
public YourFault(ErrorCode errorCode) : base(errorCode) {}
public YourFault(DateTime utc) : base(utc) {}
public YourFault(DateTime utc, EndpointAddress originator, ErrorCode errorCode, IEnumerable<Description> descriptions) : base(utc, originator, errorCode, descriptions) {}
}}

### Reading Fault Information from XML

Most commonly used on service clients leveraging the power of the library, deserialization of faults to a custom fault class is supproted as well. Follow each of the steps indicated to allow deserialization logic.

# Create your new custom fault type and subclass the BaseFaultFull class.
# Create a parameterless public constructor (this is required for deserialization of your type).
# Override the ReadStartElement method. Validate that the supplied reader is positioned at the expected start element for your type. Read the start element and perform any additional deserialization logic required (such as reading custom attributes). The base version of this method does not need to be called.
# OPTIONAL: If a custom ErrorCode is needed to be created during deserialization, override the CreateErrorCode method and perform any logic required to create the needed type(s).
# OPTIONAL: If any custom logic for creating a nested fault class or to be taken when a nested fault is encountered, override the CreateFaultCause method.
# OPTIONAL: If any additional fault content is expected to be deserialized, override the ProcessAdditionalElements method. Add any additional XmlReader operations required to read and parse the additional information. Normally this would be used to deserialize additional fault properties specific to your custom fault. By default, the base version does not support reading additional fault information and if called will throw an exception.

## Additional Features

Beyong the core feature to serialize and deserialize custom faults, hooks and types have been provided to support additional capabilities and extensions to the library.

### Creating and Using Custom ErrorCodes

The wsrf-bf:ErrorCode type supports open ended modularity for extension and customization as needed for the specific error dialect. The FaultCode<T> class was created to support the needs of strongly typed ErrorCode implementations. Out of the box it should meet the needs of most XML shapes by requiring an IXmlSerializable type in the generic type paramater. However if more control is needed, a new custom ErrorCode implementation can be created. To provide your custom logic, simply override the ProcessAdditionalElements method overloads to read and write the needed xml content. Normally the feature is coupled with a custom implementation of the BaseFaultFull.CreateErrorCode method to support deserialization.

### Deserializing Nested Faults

Most commonly supported on service clients, the wsrf-bf specification allows any number of faults to be chained together (similar in concept to the .Net or Java exception types) to supply additional information. Because deserialization into classes at runtime will require support for any possible nest fault type and content, which may or may not be represented in a client by an Xml Serializable type, the UnknownBaseFault class was created. This type is used by default when deserialization of a fault encounters a non empty FaultCause element. It supports generic untyped exploration of the XML content found in a nested fault.

In the following example, the indicated XML fragment was encountered while deserializing a fault. The code snippet displays how the XML fragement would be exposed in a fault class.
{{
<!-- additional outer xml content omitted for brevity -->
<wsbf:FaultCause>
  <InnerFault xmlns="urn:inner" customAttribute="aValue">
    <wsbf:Timestamp xmlns:wsbf="http://docs.oasis-open.org/wsrf/bf-2">2011-06-01T17:32:29.6581031Z</wsbf:Timestamp>
    <InnerContent />
  </InnerFault>
</wsbf:FaultCause>
}}

Will be turned into the following UnknownBaseFault instance
{{
// assumes that fault variable is an UnknownBaseFault class deserialized as part of a fault chain
Debug.Assert(fault.NamespaceUri == "urn:inner");
Debug.Assert(fault.LocalName == "InnerFault");
Debug.Assert(fault.XmlType == null); // Used to indicate if the XML had an xsi:type attribute value
Debug.Assert(fault.AdditionalAttributes.Count() == 1);
Debug.Assert(fault.AdditionalAttributes.First().Value == "aValue");
Debug.Assert(fault.AdditionalContent.Count() == 1);
Debug.Assert(fault.AdditionalContent.First().Name.LocalName == "InnerContent");
}}

Note: Be aware that if an UnknownBaseFault itself contains another chained fault the content will be nested in the AdditionalContent collection. No further deserialization will occur.

Customization of the nested fault type can be accomplished by overriding the BaseFaultFull.CreateFaultCause method.

### Using the wsrf-bf:BaseFault Element Directly

The specification does declare a single common fault detail element that is the basic implementation of the BaseFaultType. It contains no structural extensions or additional information. While it is recomended that custom extensions specific to your services(s) are declared and used (the wide but shallow philosophy) there are situations where the use of this element as a fault detail is desired. The library does not directly implement a type that can be used for this scenario, however, one is easy to create.

# Create a new fault type and subclass the BaseFaultFull type.
# Add any desired constructors.
# Add a new static method supporting XML schema generation. Return an XmlQualifiedName for the wsrf-bf:BaseFault element. (note: the schema can be filled from the base AcquireSchema method).
# On the class add an instance of the XmlSchemaProvider attribute indicating your new static method.