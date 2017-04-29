In order to facilitate use of the BaseFaults library and to provide a working example of code patterns leveraging the code base, the Common Faults extension was created. It provides faults for several commonly found use cases that are practically ubiquitous for non-trivial services. It provides the ability to represent when a request message does not conform to the expected or required form, a service that has maximum service execution time limits, or indicating a critical issue has been encountered.

## MessageValidationFault

This fault type is used to indicate in a common manner that there are one or more issues with the current request. Most commonly this is related to missing/unexpected XML values/elements or semantically nonsequiter or combination of exclusive inputs. While not the only way the fault contract can be used, the following examples show the most common patterns for use. Note: The examples bellow do not always show best practices for throwing fault exceptions and message operations and are presented this way for brevity.

* Performing Validation on Request Parameters
This example shows how to perform validation via imperative code to validate input parameters. While the use of an implicit Message Contract and .Net primitives is shown here, the pattern remains the same for explicit Message and Data Contracts. 
{{
[OperationContract()](OperationContract())
public bool WithdrawlFromAccount(Guid accountId, int amount)
{
  // requires amount > 0
  if (amount <= 0)
  {
    throw new FaultException<MessageValidationFault>(new MessageValidationFault("The amount to withdrawl must be at least $1"));
  }

  // Do work
  return true;
}
}}

* Performing Validation on Raw XML Messages
This example shows how to perform validation via imperative code to validate a raw XML message. 
{{
[OperationContract()](OperationContract())
public Message WithdrawlFromAccount(Message request)
{
  var settings = new XmlReaderSettings();
  settings.Schemas = GetSchemas();
  settings.ValidationEventHandler += (s, e) =>
    {
      throw new FaultException<MessageValidationFault>(new MessageValidationFault(e.Message));
    };

  var reader = XmlReader.Create(request.GetReaderAtBodyContents(), settings);

  // Do work with reader and craft response message
  return response;
}
}}

* Implicit Exception to Fault Coersion for Message Validation
For message validation the most common validation exceptions can be automatically mapped to a standardized fault construct at runtime. These examples show the common patterns altered to use this facility.
**Note:** This example requires previous configuration of the CommonFaultsHandler into the channel dispatcher to perform auto mapping functionality.

{{
// Note the use of an explicit MessageContract and use of the Data Annotations Validator utility class in this code sample

[MessageContract()](MessageContract())
public class RequestMessage
{
  public Guid AccountId { get; set; }

  [Range(1, int.MaxValue)](Range(1,-int.MaxValue))
  public int Amount { get; set; }
}

[ServiceContract()](ServiceContract())
public class MyService
{
  public ResponseMessage WithdrawlFromAccount(RequestMessage request)
  {
    // Will throw a ValidationException if the object is not valid
    Validator.ValidateObject(request, new ValidationContext(request, null, null), true);

    // Do work
    return response;
  }
}
}}

{{
[OperationContract()](OperationContract())
public Message WithdrawlFromAccount(Message request)
{
  var settings = new XmlReaderSettings();
  settings.Schemas = GetSchemas();
  settings.ValidationEventHandler += (s, e) =>
    {
      // Contains an XmlSchemaValidationException instance
      if (e.Exception != null) throw e.Exception;
    };

  var reader = XmlReader.Create(request.GetReaderAtBodyContents(), settings);

  // Do work with reader and craft response message
  return response;
}
}}

**Design Recomendation:** It is recomended that if using MessageContracts that they implement the [IValidatableObject](http://msdn.microsoft.com/en-us/library/system.componentmodel.dataannotations.ivalidatableobject.validate.aspx) interface and hide any required interaction with the validation susbsystem (or even provide their own implementation/integration with 3rd party libaries). This allows each message to be self evaluating and provides a common methodology to message validation. The use of the interface does not imply that the actual object implementing the interface is the one being validated allowing use of collaborators and polymorphic validators. Concrete examples of this pattern are beyond the scope of this document.

## ServiceUnavilableFault
Let's face it. Bugs and critical runtime errors happen. Mapping unhandled (or impossible to handle) runtime exceptions to a common fault can be used to support this common operational requirement in a standardized manner. Enabling the IncludeExceptionDetailsInFault setting should never be enabled otuside of a development environment and frankly the default WCF fallback fault lacks a particular, "professional polish". This fault can be used in its place. It is suggested that this fault should only be enabled via an IErrorHandler to perform the mapping (see "Enabling the CommonErrorHandler" section bellow).

## SLAViolationFault
Most services, especially with request-reply message exchange, have a limited time to process the request. In addition, most services involve some form of data storage query (such as SQL Server) that have an upper limit to the execution time by default. Also, user level limitations are common business requirements, providing a standardized methodology for representing violations can be accomplished with this fault contract.

* Limiting Execution Time
In this example we show how to execute an service operation within a predeteremined time period. If the operation suceeds within the time alloted the response is returned; otherwise a fault is returned. **note:** This does not show best practices with TPL, asynchronous programming, transactional systems, or data access.
{{
[OperationContract()](OperationContract())
public bool WithdrawlFromAccount(Guid accountId, int amount)
{
  var task = Task<bool>.Factory.StartNew(() => AccountDAL.Withdrawl(accountId, amount));
  task.Start();
  var completed = task.Wait(10000);
  if (!completed)
  {
    task.Cancel();
    var fault = new SlaViolationFault();
    fault.Descriptions.Add(new Description("The operation was unabled to be accomplished within the time allowed"));
    throw new FaultException<SlaViolationFault>(fault);
  }
  return task.Result;
}
}}

## Enabling the CommonErrorHandler
To facilitate the most common and universal exception to fault contract mapping that should meet the needs of most systems the CommonErrorHandler class was created. It allows the inspection of any exception raised during a service operation to be inspected and selection of the most approrpiate fault contract to be returned based on the particular exception class. In the default implementation the following exceptions (and subclasses) will be coerced into the indicated fault contract:

|| Exception type || Fault Contract ||
| [ValidationException](http://msdn.microsoft.com/en-us/library/system.componentmodel.dataannotations.validationexception.aspx) | MessageValidationFault |
| [XmlSchemaValidationException](http://msdn.microsoft.com/en-us/library/system.xml.schema.xmlschemavalidationexception.aspx) | MessageValidationFault |
| [TimeoutException](http://msdn.microsoft.com/en-us/library/system.timeoutexception.aspx) | SLAViolationFault |
| [FaultException](http://msdn.microsoft.com/en-us/library/system.servicemodel.faultexception.aspx) | returned unchanged directly to the client |
| [Exception](http://msdn.microsoft.com/en-us/library/system.exception.aspx) | ServiceUnavailableFault |

There are several approaches possible to manage IErrorHandler implementations attached to a service. The library does not provide any facility to automate or handle this configuration for you. For information on how to configure a WCF service with behaviors see [MSDN](http://msdn.microsoft.com/en-us/library/ms730137.aspx).