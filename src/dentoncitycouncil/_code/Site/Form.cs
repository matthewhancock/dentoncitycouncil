using System;
using System.Collections.Generic;

namespace Site {
    public class Form {
        //private Section[] _sections;
        //private Field[] _fields;

        protected Form(Field[] Fields) {
        }

        public static class Constants {
            public static class Parameter {
                private const string Seperator = "|";
                public static class Suffix {
                    public static class CreditCard {
                        public const string Number = Seperator + "cc-number";
                        public const string Expiration = Seperator + "cc-exp";
                        public const string Code = Seperator + "cc-code";
                        public const string AddressCity = Seperator + "cc-address-city";
                        public const string AddressState = Seperator + "cc-address-state";
                        public const string AddressPostalCode = Seperator + "cc-postalcode";
                    }
                }
            }
        }


        public abstract class Field {
            private string _key, _displayName, _cssClass;
            private bool _required;
            private KeyValuePair<string, string>[] _attributes;

            public Field(string Key, string DisplayName, bool Required = false, string CssClass = null, KeyValuePair<string, string>[] Attributes = null) {
                _key = Key;
                _displayName = DisplayName;
                _cssClass = CssClass;
                _required = Required;
                _attributes = Attributes;
            }

            public string Key { get { return _key; } }
            public string DisplayName { get { return _displayName; } }
            public string CssClass { get { return _cssClass; } }
            public bool Required { get { return _required; } }
            public KeyValuePair<string, string>[] Attributes { get { return _attributes; } }

            public abstract FieldType Type { get; }
            public abstract string Output();

            public enum FieldType { Hidden = -1, Textbox = 0, TextArea = 1, Dropdown = 2, Checkbox = 3, CreditCard = 4 }


            public class Textbox : Field {
                private string _tag;
                public Textbox(string Key, string DisplayName, bool Required = false, string CssClass = null, KeyValuePair<string, string>[] Attributes = null, string Tag = "text") : base(Key, DisplayName, Required, CssClass, Attributes) {
                    _tag = Tag;
                }

                public override FieldType Type { get { return FieldType.Textbox; } }
                public override string Output() {
                    string s = "<div class=\"description\">" + DisplayName + "</div><input type=\"" + _tag + "\" data-key=\"" + Key + "\"";
                    if (Required) {
                        s += " data-required=\"true\"";
                    }
                    if (CssClass != null) {
                        s += " class=\"" + CssClass + "\"";
                    }
                    if (Attributes != null) {
                        foreach (var a in Attributes) {
                            s += " " + a.Key + "=\"" + a.Value + "\"";
                        }
                    }
                    return s + "/>";
                }
            }
            public class Textarea : Field {
                public Textarea(string Key, string DisplayName, bool Required = false, string CssClass = null, KeyValuePair<string, string>[] Attributes = null) : base(Key, DisplayName, Required, CssClass, Attributes) { }

                public override FieldType Type { get { return FieldType.Textbox; } }
                public override string Output() {
                    string s = "<div class=\"description\">" + DisplayName + "</div><textarea data-key=\"" + Key + "\"";
                    if (Required) {
                        s += " data-required=\"true\"";
                    }
                    if (CssClass != null) {
                        s += " class=\"" + CssClass + "\"";
                    }
                    if (Attributes != null) {
                        foreach (var a in Attributes) {
                            s += " " + a.Key + "=\"" + a.Value + "\"";
                        }
                    }
                    return s + "></textarea>";
                }
            }
            public class CreditCard : Field {
                public CreditCard(string Key, string CssClass = null, KeyValuePair<string, string>[] Attributes = null) : base(Key, null, true, CssClass, Attributes) {
                }

                public override FieldType Type { get { return FieldType.CreditCard; } }
                public override string Output() {
                    string s = "<div";
                    if (CssClass != null) {
                        s += " class=\"" + CssClass + "\"";
                    }
                    if (Attributes != null) {
                        foreach (var a in Attributes) {
                            s += " " + a.Key + "=\"" + a.Value + "\"";
                        }
                    }
                    s += ">";
                    s += new Textbox(this.Key + Constants.Parameter.Suffix.CreditCard.Number, "Credit Card Number", true, null, new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("onkeydown", "return f.cc(this,event)"), new KeyValuePair<string, string>("onpaste", "return f.cc_paste(this, event)") }).Output();
                    s += $"<div class=\"f\"><div class=\"w50\">{new Textbox(this.Key + Constants.Parameter.Suffix.CreditCard.Expiration, "Expiration (MM/YY)", true, null, new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("placeholder", "MM/YY") }).Output()}</div>";
                    s += $"<div>&emsp;</div><div class=\"f1\">{new Textbox(this.Key + Constants.Parameter.Suffix.CreditCard.Code, "CCV", true).Output()}</div></div>";
                    s += new Textbox(this.Key + Constants.Parameter.Suffix.CreditCard.AddressCity, "City", true).Output();
                    s += new Dropdown(this.Key + Constants.Parameter.Suffix.CreditCard.AddressState, "State", Util.Address.AllStates, true) { Value = "NH" }.Output();
                    s += $"<div class=\"w50\">{new Textbox(this.Key + Constants.Parameter.Suffix.CreditCard.AddressPostalCode, "Zip Code", true).Output()}</div>";

                    return s + "</div>";
                }
            }
            public class Checkbox : Field {
                public Checkbox(string Key, string DisplayName, bool Required = false, string CssClass = null, KeyValuePair<string, string>[] Attributes = null) : base(Key, DisplayName, Required, CssClass, Attributes) { }

                public override FieldType Type { get { return FieldType.Checkbox; } }

                public override string Output() {
                    string id = $"chk{Guid.NewGuid().ToString("N")}_{Key}";
                    var s = $"<div class=\"form_checkbox\"><input id=\"{id}\" type=\"checkbox\" data-key=\"{Key}\"";
                    if (CssClass != null) {
                        s += " class=\"" + CssClass + "\"";
                    }
                    if (Attributes != null) {
                        foreach (var a in Attributes) {
                            s += " " + a.Key + "=\"" + a.Value + "\"";
                        }
                    }
                    s += "/>";
                    if (DisplayName != null) {
                        s += $"<label class=\"label\" for=\"{id}\">{DisplayName}</label>";
                    }

                    return s + "</div>";
                }
            }
            public class RadioSuggest : Field {
                private string[] _values;
                private bool _allowCustom;
                public RadioSuggest(string Key, string DisplayName, string[] Values, bool AllowCustom = false, string CssClass = null, KeyValuePair<string, string>[] Attributes = null) : base(Key, DisplayName, true, CssClass, Attributes) {
                    _values = Values;
                    _allowCustom = AllowCustom;
                }

                public override FieldType Type { get { return FieldType.Checkbox; } }

                public override string Output() {
                    var id = $"{Key}_{Guid.NewGuid().ToString("N")}";
                    var s = $"<div class=\"description\">{DisplayName}</div><div class=\"form_radio_suggest\">";

                    var count = 0;
                    foreach (var i in _values) {
                        if (count == 0) {
                            s += $"<input type=\"radio\" data-key=\"{Key}\" id=\"{id}_{count}\" name=\"{id}\" value=\"{i}\" checked /><label for=\"{id}_{count}\">${i}</label>";
                        } else {
                            s += $"<input type=\"radio\" data-key=\"{Key}\" id=\"{id}_{count}\" name=\"{id}\" value=\"{i}\" /><label for=\"{id}_{count}\">${i}</label>";
                        }
                        count++;
                    }
                    if (_allowCustom) {
                        s += $"<input type=\"radio\" data-key=\"{Key}\" id=\"{id}_c\" name=\"{id}\" value=\"\" /><label class=\"form_radio_suggest_custom\" for=\"{id}_c\">$<input type=\"text\" class=\"form_radio_suggest_custom_text\" onfocus=\"document.getElementById('{id}_c').checked=true;\" onkeyup=\"document.getElementById('{id}_c').value = this.value;\" /></label>";
                    }
                    return s + "</div>";
                }
            }
            public class Dropdown : Field {
                private Dictionary<string, string> _data;
                public Dropdown(string Key, string DisplayName, Dictionary<string, string> Data, bool Required = false, string CssClass = null, KeyValuePair<string, string>[] Attributes = null) : base(Key, DisplayName, Required, CssClass, Attributes) {
                    _data = Data;
                }
                public string Value { get; set; }
                public override FieldType Type { get { return FieldType.Dropdown; } }
                public override string Output() {
                    var sb = new global::System.Text.StringBuilder();
                    if (DisplayName != null) {
                        sb.Append("<div class=\"description\">" + DisplayName + "</div>");
                    }
                    sb.Append($"<select data-key=\"{Key}\"");
                    if (CssClass != null) {
                        sb.Append(" class=\"" + CssClass + "\"");
                    }
                    if (Attributes != null) {
                        foreach (KeyValuePair<string, string> a in Attributes) {
                            sb.Append(" " + a.Key + "=\"" + a.Value + "\"");
                        }
                    }
                    sb.Append(">");
                    if (!Required) {
                        if (string.IsNullOrEmpty(Value)) {
                            sb.Append("<option value=\"\" selected></option>");
                        } else {
                            sb.Append("<option value=\"\"></option>");
                        }
                    }
                    foreach (KeyValuePair<string, string> i in _data) {
                        if (i.Key == Value) {
                            sb.Append("<option value=\"" + i.Key + "\" selected>" + i.Value + "</option>");
                        } else {
                            sb.Append("<option value=\"" + i.Key + "\">" + i.Value + "</option>");
                        }
                    }
                    sb.Append("</select>");
                    return sb.ToString();
                }
            }
        }
        //public abstract class Section {
        //	private Field[] _fields;
        //	private string _name;

        //	public Section(Field[] Fields, string DisplayName = null) {
        //		_fields = Fields;
        //		_name = DisplayName;
        //	}
        //}
    }
}