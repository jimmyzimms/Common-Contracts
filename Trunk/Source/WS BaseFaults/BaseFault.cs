using System;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

namespace CommonContracts.WsBaseFaults
{
    /// <summary>
    /// Provides the minimum base common members that all <see cref="DataContractSerializer"/> based faults should provide.
    /// </summary>
    /// <remarks>Inheritors should only use <see cref="DataMember.Order"/> values greater than 1.</remarks>
    [DataContract(IsReference = false, Name = "BaseFault", Namespace = Constants.WsBaseFaultsNamespace)]
    public abstract class BaseFault
    {
        #region Fields
        
        private DateTime timestamp;
        private BaseFault faultCause;

        #endregion

        #region Constructors
        
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFault"/> class with the current <see cref="DateTime.UtcNow"/> value.
        /// </summary>
        protected BaseFault() : this(DateTime.UtcNow) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFault"/> class with the current <see cref="DateTime.UtcNow"/> value and the supplied <paramref name="faultCause"/>.
        /// </summary>
        /// <param name="faultCause">The optional cause of this fault.</param>
        protected BaseFault(BaseFault faultCause) : this(faultCause, DateTime.UtcNow)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFault"/> class with the supplied <paramref name="utc"/> value.
        /// </summary>
        /// <remarks>If the supplied date is not in UTC, the value will be coerced.</remarks>
        /// <param name="utc">The <see cref="Timestamp"/> value.</param>
        protected BaseFault(DateTime utc)
        {
            if (utc.Kind != DateTimeKind.Utc) utc = utc.ToUniversalTime();
            this.timestamp = utc;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFault"/> class with the supplied <paramref name="utc"/> and <paramref name="faultCause"/> values.
        /// </summary>
        /// <remarks>If the supplied date is not in UTC, the value will be coerced.</remarks>
        /// <param name="utc">The <see cref="Timestamp"/> value.</param>
        /// <param name="faultCause">The optional cause of this fault.</param>
        protected BaseFault(BaseFault faultCause, DateTime utc) : this(utc)
        {
            this.faultCause = faultCause;
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// Gets the time, in UTC, of the fault instance.
        /// </summary>
        /// <value>The time, in UTC, of the fault instance.</value>
        [DataMember(EmitDefaultValue = true, IsRequired = true, Name = "Timestamp", Order = 0)]
        public virtual DateTime Timestamp
        {
            get
            {
                Contract.Ensures(Contract.Result<DateTime>().Kind == DateTimeKind.Utc);
                return this.timestamp;
            }
            protected set
            {
                if (value.Kind != DateTimeKind.Utc) value = value.ToUniversalTime();
                this.timestamp = value;
            }
        }

        /// <summary>
        /// Gets or sets the cause of this fault.
        /// </summary>
        /// <value>The cause of this fault.</value>
        /// <exception cref="ArgumentException">The supplied value is the same reference as the current <see cref="BaseFault"/>.</exception>
        [DataMember(EmitDefaultValue = true, IsRequired = false, Name = "FaultCause", Order = 1)]
        public virtual BaseFault FaultCause
        {
            get { return this.faultCause; }
            set
            {
                if (ReferenceEquals(this, value)) throw new ArgumentException("You cannot nest a BaseFault with the same reference as itself as this would cause a cirular reference in the FaultCause chain.", "FaultCause");
                this.faultCause = value;
            }
        }

        #endregion
    }
}