using dentoncitycouncil;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;

namespace External {
    public class Stripe : CreditCardGateway {
        private const string API_Endpoint = "https://api.stripe.com/";
        private const string API_Version = "v1";
        private class URL {
            private static string GetPath(string Path, string ID = null, string Extension = null) {
                if (ID == null) {
                    return API_Endpoint + API_Version + "/" + Path;
                } else {
                    return API_Endpoint + API_Version + "/" + Path + "/" + ID + (Extension == null ? string.Empty : "/" + Extension);
                }
            }
            public static string Charges(string ID = null) {
                return GetPath("charges", ID);
            }
            public static string Coupons(string ID = null) {
                return GetPath("coupons", ID);
            }
            public static string Customers(string ID = null) {
                return GetPath("customers", ID);
            }
            public static string Subscription(string CustomerID) {
                return GetPath("customers", CustomerID, "subscription");
            }
            public static string Invoices(string ID = null) {
                return GetPath("invoices", ID);
            }
            public static string InvoiceItems(string ID = null) {
                return GetPath("invoiceitems", ID);
            }
            public static string Plans(string ID = null) {
                return GetPath("plans", ID);
            }
            public static string Tokens(string ID = null) {
                return GetPath("tokens", ID);
            }
            public static string Events(string ID = null) {
                return GetPath("events", ID);
            }
        }

        private class StripeJsonProperty {
            public const string ObjectType = "object";
            //-- Charge
            public const string AmountCents = "amount";
            public const string AmountRefundedCents = "amount_refunded";
            public const string Card = "card";
            public const string Created = "created";
            public const string Currency = "currency";
            public const string Customer = "customer";
            public const string Description = "description";
            public const string Disputed = "disputed";
            public const string FailureMessage = "failure_message";
            public const string FeeCents = "fee";
            public const string FeeDetails = "fee_details";
            public const string ID = "id";
            public const string InvoiceID = "invoice";
            public const string LiveMode = "livemode";
            public const string Paid = "paid";
            public const string Refunded = "refunded";
            //-- Card
            public const string AddressLine1 = "address_line1";
            public const string AddressLine1Check = "address_line1_check";
            public const string AddressLine2 = "address_line2";
            public const string AddressCity = "address_city";
            public const string AddressState = "address_state";
            public const string AddressZip = "address_zip";
            public const string AddressZipCheck = "address_zip_check";
            public const string AddressCountry = "address_country";
            public const string CardNumber = "number";
            public const string CardType = "type";
            public const string Country = "country";
            public const string CVCCode = "cvc";
            public const string CVCCheck = "cvc_check";
            public const string ExpirationMonth = "exp_month";
            public const string ExpirationYear = "exp_year";
            public const string Fingerprint = "fingerprint";
            public const string Last4Digits = "last4";
            public const string Name = "name";
            //-- Fee Detail
            public const string FeeType = "type";
            public const string FeeApplication = "application";
            //-- Error
            public const string Error = "error";
            public const string ErrorType = "type";
            public const string ErrorMessage = "message";
            public const string ErrorCode = "code";
            public const string ErrorParam = "param";
        }
        private class StripeJsonValue {
            //-- object
            public const string Charge = "charge";
            //-- card.type
            public const string Visa = "Visa";
            public const string AmericanExpress = "American Express";
            public const string MasterCard = "MasterCard";
            public const string JCB = "JCB";
            public const string DinersClub = "Diners Club";
            public const string Unknown = "Unknown";
            //-- check
            public const string Pass = "pass";
            public const string Fail = "fail";
            public const string Unchecked = "unchecked";
            //-- error
            public class ErrorType {
                public const string InvalidRequest = "invalid_request_error";
                public const string APIError = "api_error";
                public const string CardError = "card_error";
                public class Card {
                    public const string InvalidNumber = "invalid_number";
                    public const string InvalidExpirationMonth = "invalid_expiry_month";
                    public const string InvalidExpirationYear = "invalid_expiry_year";
                    public const string InvalidCVC = "invalid_cvc";
                    public const string IncorrectNumber = "incorrect_number";
                    public const string ExpiredCard = "expired_card";
                    public const string IncorrectCVC = "incorrect_cvc";
                    public const string IncorrectZip = "incorrect_cvc";
                    public const string Declined = "card_declined";
                    //-- Stripe specific, unlikely unless charging customer instead of card
                    public const string CustomerHasNoCard = "missing";
                    public const string ProcessingError = "processing_error";
                    public const string RateLimit = "rate_limit";
                }
            }
        }
        public override CreditCardGatewayResponse Charge(double ChargeAmount, /*System.Currency Currency,*/ string Description, string CreditCardNumber, byte ExpirationMonth, short ExpirationYear, string CVCCode, string CardholderName, string AddressLine1, string AddressLine2, string AddressState, string AddressZip, string AddressCountry) {
            Request.Charge r = new Request.Charge();
            r.AddParameter(StripeJsonProperty.AmountCents, ((int)(ChargeAmount * 100)).ToString());
            r.AddParameter(StripeJsonProperty.Currency, "USD"); // Currency.ISO);
            r.AddParameter(StripeJsonProperty.Description, Description);
            //-- Card
            r.AddDictionaryParameter(StripeJsonProperty.Card, StripeJsonProperty.CardNumber, CreditCardNumber);
            r.AddDictionaryParameter(StripeJsonProperty.Card, StripeJsonProperty.ExpirationMonth, ExpirationMonth.ToString());
            r.AddDictionaryParameter(StripeJsonProperty.Card, StripeJsonProperty.ExpirationYear, ExpirationYear.ToString());
            r.AddDictionaryParameter(StripeJsonProperty.Card, StripeJsonProperty.CVCCode, CVCCode);
            r.AddDictionaryParameter(StripeJsonProperty.Card, StripeJsonProperty.Name, CardholderName);
            //r.AddDictionaryParameter(StripeJsonProperty.Card, StripeJsonProperty.AddressLine1, AddressLine1);
            //if (AddressLine2 != null) {
            //    r.AddDictionaryParameter(StripeJsonProperty.Card, StripeJsonProperty.AddressLine2, AddressLine2);
            //}
            //r.AddDictionaryParameter(StripeJsonProperty.Card, StripeJsonProperty.AddressState, AddressState);
            r.AddDictionaryParameter(StripeJsonProperty.Card, StripeJsonProperty.AddressZip, AddressZip);
            //r.AddDictionaryParameter(StripeJsonProperty.Card, StripeJsonProperty.AddressCountry, AddressCountry);

            return r.GetResponse();
        }
        private class Request {
            private string _url;
            private List<KeyValuePair<string, string>> _parameters;
            protected Request(string URL) {
                _url = URL;
            }
            public void AddParameter(string Key, string Value) {
                if (_parameters == null) {
                    _parameters = new List<KeyValuePair<string, string>>();
                }
                _parameters.Add(new KeyValuePair<string, string>(Key, Value));
            }
            public void AddDictionaryParameter(string DictionaryKey, string Key, string Value) {
                AddParameter(DictionaryKey + "[" + Key + "]", Value);
            }
            public Response GetResponse(string HttpMethod = "POST") {
                WebRequest r = WebRequest.Create(_url + GetQueryString());
                r.Method = HttpMethod;
                r.Credentials = new NetworkCredential(Application.Stripe.SecretKey, string.Empty);

                string s = null;
                try {
                    WebResponse rs = r.GetResponse();
                    s = new System.IO.StreamReader(rs.GetResponseStream()).ReadToEnd();
                } catch (WebException ex) {
                    s = new System.IO.StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                }

                if (s != null) {
                    return Response.ParseJson(s);
                } else {
                    return null;
                }
            }
            private string GetQueryString() {
                string result = string.Empty;
                if (_parameters != null) {
                    bool started = false;
                    foreach (var i in _parameters) {
                        if (!started) {
                            result = "?";
                            started = true;
                        } else {
                            result += "&";
                        }
                        result += i.Key + "=" + Uri.EscapeDataString(i.Value);
                    }
                }
                return result;
            }
            public class Charge : Request {
                public Charge(string ID = null) : base(URL.Charges(ID)) {
                }
            }
        }
        public class Response : CreditCardGatewayResponse {
            private StripeType _o = null;
            private StripeException _e = null;
            private Response(StripeType o) : base(true) {
                _o = o;
            }
            private Response(StripeException e) : base(false) {
                _e = e;
            }
            public static Response ParseJson(string Response) {
                JObject js = JObject.Parse(Response);
                var o = js[StripeJsonProperty.ObjectType];
                if (o != null) {
                    return new Response(new StripeType.Charge(js));
                } else {
                    var e = js[StripeJsonProperty.Error];
                    if (e != null) {
                        var error = (string)e[StripeJsonProperty.ErrorType];
                        switch (error) {
                            case StripeJsonValue.ErrorType.APIError:
                                return new Response(new StripeException.APIError((JObject)e));
                            case StripeJsonValue.ErrorType.CardError:
                                return new Response(new StripeException.CardError((JObject)e));
                            case StripeJsonValue.ErrorType.InvalidRequest:
                                return new Response(new StripeException.InvalidRequestError((JObject)e));
                        }
                    }
                }
                return null;
            }

            public StripeException Error { get { return _e; } }

            public abstract class StripeType {
                protected string _id;
                protected string _objectType;
                public string ID {
                    get { return _id; }
                }
                public string ObjectType {
                    get { return _objectType; }
                }
                public StripeType(JObject json) {
                    _id = json[StripeJsonProperty.ID]?.ToString();
                    _objectType = json[StripeJsonProperty.ObjectType]?.ToString();
                }
                public class Charge : StripeType {
                    private string _currency;
                    private string _customerID;
                    private string _description;
                    private string _failureMessage;
                    private string _invoiceID;
                    private bool _livemode;
                    private bool _disputed;
                    private bool _paid;
                    private bool _refunded;
                    private int _amountCents;
                    private int _feeCents;
                    private int _amountRefundedCents;
                    private DateTime _created;
                    private Card _card;
                    private FeeDetails _feeDetails;
                    public Charge(JObject json) : base(json) {

                        foreach (var i in json) {
                            switch (i.Key) {
                                case StripeJsonProperty.Currency:
                                    _currency = i.Value.ToString();
                                    break;
                                case StripeJsonProperty.Customer:
                                    _customerID = i.Value.ToString();
                                    break;
                                case StripeJsonProperty.Description:
                                    _description = i.Value.ToString();
                                    break;
                                case StripeJsonProperty.FailureMessage:
                                    _failureMessage = i.Value.ToString();
                                    break;
                                case StripeJsonProperty.InvoiceID:
                                    _invoiceID = i.Value.ToString();
                                    break;
                                case StripeJsonProperty.LiveMode:
                                    _livemode = (bool)i.Value;
                                    break;
                                case StripeJsonProperty.Disputed:
                                    _disputed = (bool)i.Value;
                                    break;
                                case StripeJsonProperty.Paid:
                                    _paid = (bool)i.Value;
                                    break;
                                case StripeJsonProperty.Refunded:
                                    _refunded = (bool)i.Value;
                                    break;
                                case StripeJsonProperty.AmountCents:
                                    _amountCents = (int)i.Value;
                                    break;
                                case StripeJsonProperty.FeeCents:
                                    _feeCents = (int)i.Value;
                                    break;
                                case StripeJsonProperty.AmountRefundedCents:
                                    _amountRefundedCents = (int)i.Value;
                                    break;
                                case StripeJsonProperty.Created:
                                    _created = (new DateTime(1970, 1, 1, 0, 0, 0, 0)).AddSeconds((int)json[StripeJsonProperty.Created]);
                                    break;
                                case StripeJsonProperty.Card:
                                    _card = new Card((JObject)i.Value);
                                    break;
                                case StripeJsonProperty.FeeDetails:
                                    _feeDetails = new FeeDetails((JArray)i.Value);
                                    break;
                            }
                        }
                    }
                }
                public class FeeDetail : StripeType {
                    private int _amountCents;
                    private string _currency;
                    private string _type;
                    private string _application;
                    private string _description;
                    public FeeDetail(JObject json) : base(json) {
                        _amountCents = (int)json[StripeJsonProperty.AmountCents];
                        _currency = (string)json[StripeJsonProperty.Currency];
                        _type = (string)json[StripeJsonProperty.FeeType];
                        _application = (string)json[StripeJsonProperty.FeeApplication];
                        _description = (string)json[StripeJsonProperty.Description];
                    }
                }
                public class FeeDetails : List<FeeDetail> {
                    public FeeDetails(JArray json) {
                        foreach (JObject i in json) {
                            this.Add(new FeeDetail(i));
                        }
                    }
                }
                public class Card : StripeType {
                    private string _fingerprint;
                    private string _last4;
                    private string _cardType;
                    private string _addressCity;
                    private string _addressCountry;
                    private string _addressLine1;
                    private string _addressLine1Check;
                    private string _addressLine2;
                    private string _addressState;
                    private string _addressZip;
                    private string _addressZipCheck;
                    private string _country;
                    private string _cvcCheck;
                    private string _name;
                    private Int16 _expirationMonth;
                    private Int16 _expirationYear;
                    public Card(JObject json) : base(json) {
                        foreach (var i in json) {
                            switch (i.Key) {
                                case StripeJsonProperty.Fingerprint:
                                    _fingerprint = i.Value.ToString();
                                    break;
                                case StripeJsonProperty.Last4Digits:
                                    _last4 = i.Value.ToString();
                                    break;
                                case StripeJsonProperty.CardType:
                                    _cardType = i.Value.ToString();
                                    break;
                                case StripeJsonProperty.AddressCity:
                                    _addressCity = i.Value.ToString();
                                    break;
                                case StripeJsonProperty.AddressCountry:
                                    _addressCountry = i.Value.ToString();
                                    break;
                                case StripeJsonProperty.AddressLine1:
                                    _addressLine1 = i.Value.ToString();
                                    break;
                                case StripeJsonProperty.AddressLine1Check:
                                    _addressLine1Check = i.Value.ToString();
                                    break;
                                case StripeJsonProperty.AddressLine2:
                                    _addressLine2 = i.Value.ToString();
                                    break;
                                case StripeJsonProperty.AddressState:
                                    _addressState = i.Value.ToString();
                                    break;
                                case StripeJsonProperty.AddressZip:
                                    _addressZip = i.Value.ToString();
                                    break;
                                case StripeJsonProperty.AddressZipCheck:
                                    _addressZipCheck = i.Value.ToString();
                                    break;
                                case StripeJsonProperty.Country:
                                    _country = i.Value.ToString();
                                    break;
                                case StripeJsonProperty.CVCCheck:
                                    _cvcCheck = i.Value.ToString();
                                    break;
                                case StripeJsonProperty.Name:
                                    _name = i.Value.ToString();
                                    break;
                                case StripeJsonProperty.ExpirationMonth:
                                    _expirationMonth = (short)i.Value;
                                    break;
                                case StripeJsonProperty.ExpirationYear:
                                    _expirationYear = (short)i.Value;
                                    break;
                            }
                        }
                    }
                }
            }
            public abstract class StripeException {
                private string _type;
                private string _message;
                private string _code;
                private string _param;
                public StripeException(JObject json) {
                    foreach (var i in json) {
                        switch (i.Key) {
                            case StripeJsonProperty.ErrorType:
                                _type = i.Value.ToString();
                                break;
                            case StripeJsonProperty.ErrorMessage:
                                _message = i.Value.ToString();
                                break;
                            case StripeJsonProperty.ErrorCode:
                                _code = i.Value.ToString();
                                break;
                            case StripeJsonProperty.ErrorParam:
                                _param = i.Value.ToString();
                                break;
                        }
                    }
                }

                public string Message { get { return _message; } }

                public class InvalidRequestError : StripeException {
                    public InvalidRequestError(JObject json) : base(json) {
                    }
                }
                public class APIError : StripeException {
                    public APIError(JObject json) : base(json) {
                    }
                }
                public class CardError : StripeException {
                    public CardError(JObject json) : base(json) {
                    }
                }
            }
        }
    }
}
