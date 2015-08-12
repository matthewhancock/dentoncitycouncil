using Microsoft.AspNet.Http;
using System;
using System.Threading.Tasks;

namespace dentoncitycouncil.Handlers {
    public class Post {
        public static async Task ProcessRequestAsync(HttpContext Context, string Action) {
            var form = Context.Request.Form;
            Context.Response.ContentType = "text/javascript";

            switch (Action) {
                case Forms.HelpElect.Action.Donate:
                    await Process.Donate(form, Context);
                    break;
                case Forms.HelpElect.Action.Support:
                    await Process.Support(form, Context);
                    break;
            }
        }

        public static class Process {
            public static async Task Donate(IFormCollection Form, HttpContext Context) {
                var ok = true; string error_message = null;
                var name = Form[Forms.HelpElect.Keys.Name];
                var email = Form[Forms.HelpElect.Keys.Email];
                double amount = -1;
                double.TryParse(Form[Forms.HelpElect.Keys.Amount], out amount);
                if (amount < 5) {
                    ok = false;
                    error_message = "Sorry, due to credit card processing fees the minimum contribution is $5.";
                }
                var cc_number = Form[Forms.HelpElect.Keys.CreditCard + Site.Form.Constants.Parameter.Suffix.CreditCard.Number].Replace(" ", string.Empty);
                string cc_exp = Form[Forms.HelpElect.Keys.CreditCard + Site.Form.Constants.Parameter.Suffix.CreditCard.Expiration];
                byte cc_month = 0; short cc_year = 0;
                if (cc_exp != null && cc_exp.Length >= 4) {
                    byte.TryParse(cc_exp.Substring(0, 2), out cc_month);
                    short.TryParse(cc_exp.Substring(cc_exp.Length - 2), out cc_year);
                    if (cc_month >= 1 && cc_month <= 12 && cc_year > 14) {
                        // valid expiration dates
                    } else {
                        ok = false;
                        if (error_message == null) {
                            error_message = "Please enter a valid expiration date. (e.g. \"08/16\")";
                        } else {
                            error_message = "<br />Please enter a valid expiration date. (e.g. \"08/16\")";
                        }
                    }
                } else {
                    ok = false;
                    if (error_message == null) {
                        error_message = "Please enter a valid expiration date. (e.g. \"08/16\")";
                    } else {
                        error_message = "<br />Please enter a valid expiration date. (e.g. \"08/16\")";
                    }
                }
                var cc_cvv = Form[Forms.HelpElect.Keys.CreditCard + Site.Form.Constants.Parameter.Suffix.CreditCard.Code];
                var cc_city = Form[Forms.HelpElect.Keys.CreditCard + Site.Form.Constants.Parameter.Suffix.CreditCard.AddressCity];
                var cc_state = Form[Forms.HelpElect.Keys.CreditCard + Site.Form.Constants.Parameter.Suffix.CreditCard.AddressState];
                var cc_zip = Form[Forms.HelpElect.Keys.CreditCard + Site.Form.Constants.Parameter.Suffix.CreditCard.AddressPostalCode];

                if (ok) {
                    External.Stripe.Response stripe = (External.Stripe.Response) new External.Stripe().Charge(amount, $"Donation from {name}, {email}", cc_number, cc_month, cc_year, cc_cvv, name, null, null, cc_state, cc_zip, null);
                    if (stripe != null) {
                        if (stripe.Success) {
                            await Context.Response.WriteAsync(Response.Substitute(Forms.HelpElect.HtmlID.DonateFormContainer, $"<div class=\"tac\">Thanks! Your contribution has been received and a confirmation email will be sent to {email} shortly.</div>"));

                            var confirmation = new External.Email() {To = email, Subject = "Thank you for your contribution to Denton for City Council",
                            Body = $"Thank you for your generous contribution of ${amount} to my campaign. It means a lot to have your support.\r\n\r\nThanks again,\r\nJosh Denton"};
                            var notification = new External.Email() { To = Application.HelpElect.Donate.ConfirmationEmail, Subject = $"Denton for City Council: New Donation ${amount}",
                                Body = $"A new contribution was received from: {name}\r\nAmount: ${amount}\r\n\r\nEmail Address: {email}\r\nCity: {cc_city}\r\nState: {cc_state}\r\nZip Code: {cc_zip}" };

                            await Task.WhenAll(new Task[] { confirmation.Send(), notification.Send() });
                        } else {
                            if (stripe.Error != null) {
                                if (stripe.Error is External.Stripe.Response.StripeException.APIError) {
                                    await Context.Response.WriteAsync(Response.Error("Sorry, there was an error with the credit card processor. Please try again shortly.", true));
                                } else if (stripe.Error is External.Stripe.Response.StripeException.CardError) {
                                    await Context.Response.WriteAsync(Response.Error("Sorry, there was an error with the credit card. Message: " + stripe.Error.Message, true));
                                } else if (stripe.Error is External.Stripe.Response.StripeException.InvalidRequestError) {
                                    await Context.Response.WriteAsync(Response.Error("Sorry, there was an error processing your credit card.", true));
                                }
                            } else {
                                await Context.Response.WriteAsync(Response.Error("Sorry, there was an error processing your credit card.", true));
                            }
                        }
                    } else {
                        await Context.Response.WriteAsync(Response.Error("Sorry, there was an error processing your credit card.", true));
                    }
                } else {
                    await Context.Response.WriteAsync(Response.Error(error_message, true));
                }
            }
            public static async Task Support(IFormCollection Form, HttpContext Context) {
                var name = Form[Forms.HelpElect.Keys.Name];
                var email = Form[Forms.HelpElect.Keys.Email];
                var address = Form[Forms.HelpElect.Keys.Address];
                bool bumpersticker = false, yardsign = false;
                bool.TryParse(Form[Forms.HelpElect.Keys.BumperSticker], out bumpersticker);
                bool.TryParse(Form[Forms.HelpElect.Keys.YardSign], out yardsign);

                var message = "Thanks! Your information has been received.";
                if (bumpersticker) {
                    if (yardsign) {
                        // bumpersticker AND yardsign
                        message = "Thank you for support, I will reach out to you this week about delivering your Denton for City Council yard sign and bumper sticker soon.";
                    } else {
                        // bumpersticker BUT NOT yardsign
                        message = "Thank you for support, I will reach out to you this week about delivering your Denton for City Council bumper sticker soon.";
                    }
                } else {
                    if (yardsign) {
                        // NOT bumpersticker BUT yardsign
                        message = "Thank you for support, I will reach out to you this week about delivering your Denton for City Council yard sign soon.";
                    }
                }

                await new External.Email() { To = Application.HelpElect.Support.Email, Subject = "Denton for City Council: New Supporter",
                    Body = $"A new user, {name}, has pledged support on the website.\r\n\r\nEmail: {email}\r\nAddress: {address}\r\n\r\nYard Sign: {(yardsign ? "Yes" : "No")}\r\nBumper Sticker: {(bumpersticker ? "Yes" : "No")}"
                }.Send();
                await Context.Response.WriteAsync(Response.Substitute(Forms.HelpElect.HtmlID.SupportFormContainer, $"<div class=\"tac\">{message}</div>"));
            }
        }


        public static class Response {
            public static string Form(string Content) {
                return "{\"ok\":true,\"form\":\"" + Fix(Content) + "\"}";
            }
            public static string Substitute(string ID, string Value) {
                return "{\"ok\":true,\"reenable\":true,\"substitute\":{\"id\":\"" + ID + "\", \"value\":\"" + Fix(Value) + "\"}}";
            }
            public static string Push(string Href, string Title, string Content, string HrefID = null) {
                if (Href != null) {
                    return "{\"ok\":true,\"push\":{\"state\":{\"href\":\"" + Href + "\", \"title\":\"" + Fix(Title) + "\", \"content\":\"" + Fix(Content) + "\", \"hrefid\":\"" + Fix(HrefID) + "\"}}}";
                } else {
                    return "{\"ok\":true,\"push\":{\"state\":{\"href\":\"" + Href + "\", \"title\":\"" + Fix(Title) + "\", \"content\":\"" + Fix(Content) + "\"}}}";
                }
            }
            public static string OK(string Message) {
                return "{\"ok\":true,\"reenable\":true,\"message\":\"" + Fix(Message) + "\"}";
            }
            public static string Error(string Message, bool Reenable = false) {
                if (Reenable) {
                    return "{\"ok\":false,\"message\":\"" + Fix(Message) + "\",\"reenable\":true}";
                } else {
                    return "{\"ok\":false,\"message\":\"" + Fix(Message) + "\"}";
                }
            }
            public static string Fix(string s) {
                return Util.Json.Fix(s);
            }
        }
    }
}