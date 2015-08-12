using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace External {
    public abstract class CreditCardGateway {
        public abstract CreditCardGatewayResponse Charge(double ChargeAmount, /*System.Currency Currency,*/ string Description, string CreditCardNumber, byte ExpirationMonth, Int16 ExpirationYear, string CVCCode, string CardholderName, string AddressLine1, string AddressLine2, string AddressState, string AddressZip, string AddressCountry);

        public abstract class CreditCardGatewayResponse : Util.Result {
            public CreditCardGatewayResponse(bool Result) : base(Result) {
            }
        }
    }
}
