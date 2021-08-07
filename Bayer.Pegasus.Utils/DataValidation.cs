using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bayer.Pegasus.Utils
{
    public class DataValidation
    {
        String Field = Bayer.Pegasus.Utils.Resources.Validation.TheField;
        String Filled = Bayer.Pegasus.Utils.Resources.Validation.MustBeFilled;
        String Decimal = Bayer.Pegasus.Utils.Resources.Validation.MustBeValidDecimalValue;
        String Integer = Bayer.Pegasus.Utils.Resources.Validation.MustBeValidIntegerValue;
        String BiggerThan = Bayer.Pegasus.Utils.Resources.Validation.MustBeBiggerThan;

        String Date = Bayer.Pegasus.Utils.Resources.Validation.MustBeValidDate;
        String Between = Bayer.Pegasus.Utils.Resources.Validation.Between;
        String And = Bayer.Pegasus.Utils.Resources.Validation.And;
        String Characters = Bayer.Pegasus.Utils.Resources.Validation.Characters;
        String Time = Bayer.Pegasus.Utils.Resources.Validation.MustBeValidTime;

        public DataValidation()
        {
            this.FeedBackService = new FeedbackService();
        }

        public DataValidation(FeedbackService feedbackService)
        {
            this.FeedBackService = feedbackService;
        }

        public FeedbackService FeedBackService
        {
            get;
            private set;
        }

        public bool ValidateTime(string fieldName, bool required, object o, string fieldDescription)
        {
            string valueToValidate = "";

            if (o != null)
            {
                valueToValidate = o.ToString();
            }

            if (required && String.IsNullOrEmpty(valueToValidate))
            {
                FeedBackService.AddCustomError(fieldName, Field + fieldDescription + Filled);

                return false;
            }

            if (o == null)
                return true;

            if (!String.IsNullOrEmpty(o.ToString()))
            {
                DateTime datevalue;

                CultureInfo ptBR = new CultureInfo("pt-BR");
                bool parse = (DateTime.TryParseExact(o.ToString(), "HH:mm", ptBR,
                     DateTimeStyles.None, out datevalue));

                if (!parse)
                {
                    FeedBackService.AddCustomError(fieldName, Field + fieldDescription + Time);

                    return false;
                }
            }

            return true;
        }

        public bool ValidateEmail(string fieldName, bool required, object o, string fieldDescription, int min, int max)
        {
            bool ok = ValidateString(fieldName, required, o, fieldDescription, min, max);

            if (!ok)
                return ok;

            if (!required && String.IsNullOrEmpty((string)o))
                return true;

            System.Text.RegularExpressions.Regex rgx = new System.Text.RegularExpressions.Regex(RegularExpressions.Email);


            ok = rgx.IsMatch(o.ToString());

            if (!ok)
            {
                FeedBackService.AddCustomError(fieldName, Field + fieldDescription + Bayer.Pegasus.Utils.Resources.Validation.MustBeValidEmail);
                return false;
            }

            return true;
        }

        public bool ValidateCEP(string fieldName, bool required, object o, string fieldDescription)
        {
            bool ok = ValidateString(fieldName, required, o, fieldDescription, 9, 9);

            if (!ok)
                return ok;

            if (!required && String.IsNullOrEmpty((string)o))
                return true;


            System.Text.RegularExpressions.Regex rgx = new System.Text.RegularExpressions.Regex(RegularExpressions.CEP);


            ok = rgx.IsMatch(o.ToString());

            if (!ok)
            {
                FeedBackService.AddCustomError(fieldName, Field + fieldDescription + Bayer.Pegasus.Utils.Resources.Validation.MustBeValidZipCode);
                return false;
            }

            return true;
        }

        public bool ValidateCPF(string fieldName, bool required, object o, string fieldDescription)
        {
            bool ok = ValidateString(fieldName, required, o, fieldDescription, 1, 20);

            if (!ok)
                return ok;

            if (!required && String.IsNullOrEmpty((string)o))
                return true;


            System.Text.RegularExpressions.Regex rgx = new System.Text.RegularExpressions.Regex(RegularExpressions.CPF);


            ok = rgx.IsMatch(o.ToString());

            if (!ok)
            {
                FeedBackService.AddCustomError(fieldName, Field + fieldDescription + Bayer.Pegasus.Utils.Resources.Validation.MustBeValidCPF);
                return false;
            }

            return true;
        }

        public bool ValidateCNPJ(string fieldName, bool required, object o, string fieldDescription)
        {
            bool ok = ValidateString(fieldName, required, o, fieldDescription, 1, 20);

            if (!ok)
                return ok;

            if (!required && String.IsNullOrEmpty((string)o))
                return true;


            System.Text.RegularExpressions.Regex rgx = new System.Text.RegularExpressions.Regex(RegularExpressions.CNPJ);


            ok = rgx.IsMatch(o.ToString());

            if (!ok)
            {
                FeedBackService.AddCustomError(fieldName, Field + fieldDescription + Bayer.Pegasus.Utils.Resources.Validation.MustBeValidCNPJ);
                return false;
            }

            return true;
        }

        public bool ValidatePhone(string fieldName, bool required, object o, string fieldDescription)
        {
            bool ok = ValidateString(fieldName, required, o, fieldDescription, 1, 20);

            if (!ok)
                return ok;

            if (!required && String.IsNullOrEmpty((string)o))
                return true;

            System.Text.RegularExpressions.Regex rgx = new System.Text.RegularExpressions.Regex(RegularExpressions.Telefone);

            ok = rgx.IsMatch(o.ToString());

            if (!ok)
            {
                FeedBackService.AddCustomError(fieldName, Field + fieldDescription + Bayer.Pegasus.Utils.Resources.Validation.MustBeValidPhoneNumber);
                return false;
            }

            return true;
        }

        public bool ValidateBoolean(string fieldName, bool required, object o, dynamic fieldDescription)
        {
            return ValidateBoolean(fieldName, required, o, Convert.ToString(fieldDescription));
        }

        public bool ValidateBoolean(string fieldName, bool required, object o, string fieldDescription)
        {
            string valueToValidate = "";

            if (o != null)
            {
                valueToValidate = o.ToString();
            }

            if (required && String.IsNullOrEmpty(valueToValidate))
            {
                FeedBackService.AddCustomError(fieldName, Field + fieldDescription + Filled);

                return false;
            }


            if (o == null)
                return true;

            if (!String.IsNullOrEmpty(o.ToString()))
            {
                bool value;
                bool parse = bool.TryParse(o.ToString(), out value);

                if (!parse)
                {
                    int n;

                    parse = int.TryParse(o.ToString(), out n);

                    if (parse)
                    {
                        if (n >= 0 && n <= 1)
                        {
                            return true;
                        }
                    }

                    FeedBackService.AddCustomError(fieldName, Field + fieldDescription + Decimal);

                    return false;
                }
            }

            return true;
        }

        public bool ValidateDecimal(string fieldName, bool required, object o, dynamic fieldDescription)
        {
            return ValidateDecimal(fieldName, required, o, Convert.ToString(fieldDescription));
        }

        public bool ValidateDecimal(string fieldName, bool required, object o, string fieldDescription)
        {
            string valueToValidate = "";

            if (o != null)
            {
                valueToValidate = o.ToString();
            }

            if (required && String.IsNullOrEmpty(valueToValidate))
            {
                FeedBackService.AddCustomError(fieldName, Field + fieldDescription + Filled);

                return false;
            }

            if (o == null)
                return true;

            if (!String.IsNullOrEmpty(o.ToString()))
            {
                decimal n;
                bool parse = decimal.TryParse(o.ToString(), out n);

                if (!parse)
                {
                    FeedBackService.AddCustomError(fieldName, Field + fieldDescription + Decimal);

                    return false;
                }
            }

            return true;
        }

        public bool ValidateInteger(string fieldName, bool required, object o, dynamic fieldDescription)
        {
            return ValidateInteger(fieldName, required, o, Convert.ToString(fieldDescription));
        }


        public bool ValidateIntegerGreaterThanZero(string fieldName, bool required, object o, dynamic fieldDescription)
        {
            Int64? value = o as Int64?;

            if (value.HasValue)
            {
                if (value < 1)
                {
                    return ValidateString(fieldName, required, "", fieldDescription, 0, 10);
                }
            }


            return ValidateInteger(fieldName, required, o, Convert.ToString(fieldDescription));
        }

        public bool ValidateIntegerGreaterThanOne(string fieldName, bool required, object o, dynamic fieldDescription, long max)
        {
            Int64? value = o as Int64?;

            if (value.HasValue)
            {
                if (value < 0)
                {

                    FeedBackService.AddCustomError(fieldName, Field + fieldDescription + Integer.Replace(".", "") + And + BiggerThan + " 0.");

                    return false;



                }
                else
                {
                    if (value == 0)
                    {
                        return ValidateString(fieldName, required, "", fieldDescription, 0, 10);

                    }
                }
            }


            return ValidateInteger(fieldName, required, o, Convert.ToString(fieldDescription));
        }


        public bool ValidateInteger(string fieldName, bool required, object o, string fieldDescription)
        {
            string valueToValidate = "";

            if (o != null)
            {
                valueToValidate = o.ToString();
            }

            if (required && String.IsNullOrEmpty(valueToValidate))
            {
                FeedBackService.AddCustomError(fieldName, Field + fieldDescription + Filled);

                return false;
            }

            if (o == null)
                return true;

            if (!String.IsNullOrEmpty(o.ToString()))
            {
                int n;
                bool parse = int.TryParse(o.ToString(), out n);

                if (!parse)
                {
                    FeedBackService.AddCustomError(fieldName, Field + fieldDescription + Integer);

                    return false;
                }
            }

            return true;
        }

        public bool ValidateMonth(string fieldName, bool required, object o, string fieldDescription)
        {
            string valueToValidate = "";

            if (o != null)
            {
                valueToValidate = o.ToString();
            }

            if (required && String.IsNullOrEmpty(valueToValidate))
            {
                FeedBackService.AddCustomError(fieldName, Field + fieldDescription + Filled);

                return false;
            }

            if (o == null)
                return true;


            if (!String.IsNullOrEmpty(o.ToString()))
            {

                DateTime n;
                var parse = DateTime.TryParseExact(o.ToString(), "MM/yyyy",
                                    new CultureInfo("pt-br"),
                                    DateTimeStyles.None,
                                    out n);
                
                if (!parse)
                {
                    FeedBackService.AddCustomError(fieldName, Field + fieldDescription + Date);

                    return false;
                }
            }


            return true;
             

        }

        public bool ValidateDate(string fieldName, bool required, object o, string fieldDescription)
        {
            string valueToValidate = "";

            if (o != null)
            {
                valueToValidate = o.ToString();
            }

            if (required && String.IsNullOrEmpty(valueToValidate))
            {
                FeedBackService.AddCustomError(fieldName, Field + fieldDescription + Filled);

                return false;
            }

            if (o == null)
                return true;

            if (!String.IsNullOrEmpty(o.ToString()))
            {
                DateTime n;
                bool parse = DateTime.TryParse(o.ToString(), out n);

                if (!parse)
                {
                    FeedBackService.AddCustomError(fieldName, Field + fieldDescription + Date);

                    return false;
                }
                else
                {
                    try
                    {
                        System.Data.SqlTypes.SqlDateTime sdt = new System.Data.SqlTypes.SqlDateTime(n);
                    }
                    catch (System.Data.SqlTypes.SqlTypeException ex)
                    {
                        FeedBackService.AddCustomError(fieldName, Field + fieldDescription + Between + "01/01/1753" + And + "31/12/9999");

                        return false;
                    }
                }
            }

            return true;
        }

        public bool ValidateString(string fieldName, bool required, object o, dynamic fieldDescription, int min, int max)
        {
            return ValidateString(fieldName, required, o, Convert.ToString(fieldDescription), min, max);
        }


        public bool ValidateArray(string fieldName, bool required, Newtonsoft.Json.Linq.JArray values, string fieldDescription)
        {
            var list = Bayer.Pegasus.Utils.JsonUtils.GetListFilterValues(values);

            if (list.Count == 0 && required) {
                FeedBackService.AddCustomError(fieldName, Field + fieldDescription + Filled);

                return false;
            }

            return true;
        }

        public bool ValidateString(string fieldName, bool required, object o, string fieldDescription, int min, int max)
        {
            string valueToValidate = "";

            if (o != null)
            {
                valueToValidate = o.ToString();
            }

            if (required && String.IsNullOrEmpty(valueToValidate))
            {
                FeedBackService.AddCustomError(fieldName, Field + fieldDescription + Filled);

                return false;
            }

            if (String.IsNullOrEmpty(valueToValidate))
            {
                return true;
            }

            if (valueToValidate.Length > max || valueToValidate.Length < min)
            {
                FeedBackService.AddCustomError(fieldName, Field + fieldDescription + Between + min + And + max + Characters);

                return false;
            }

            valueToValidate = valueToValidate.Replace(Environment.NewLine, "");
            valueToValidate = valueToValidate.Replace("\n", "");
            valueToValidate = valueToValidate.Replace("+", "");
            valueToValidate = valueToValidate.Replace("_", "");
            valueToValidate = valueToValidate.Replace(")", "");
            valueToValidate = valueToValidate.Replace("(", "");
            valueToValidate = valueToValidate.Replace("*", "");
            valueToValidate = valueToValidate.Replace("&", "");
            valueToValidate = valueToValidate.Replace("¨", "");
            valueToValidate = valueToValidate.Replace("%", "");
            valueToValidate = valueToValidate.Replace("$", "");
            valueToValidate = valueToValidate.Replace("#", "");
            valueToValidate = valueToValidate.Replace("@", "");
            valueToValidate = valueToValidate.Replace("!", "");
            valueToValidate = valueToValidate.Replace("?", "");
            valueToValidate = valueToValidate.Replace(":", "");
            valueToValidate = valueToValidate.Replace("^", "");
            valueToValidate = valueToValidate.Replace("á", "");
            valueToValidate = valueToValidate.Replace("Á", "");
            valueToValidate = valueToValidate.Replace("ã", "");
            valueToValidate = valueToValidate.Replace("Ã", "");
            valueToValidate = valueToValidate.Replace("â", "");
            valueToValidate = valueToValidate.Replace("Â", "");
            valueToValidate = valueToValidate.Replace("à", "");
            valueToValidate = valueToValidate.Replace("À", "");
            valueToValidate = valueToValidate.Replace("é", "");
            valueToValidate = valueToValidate.Replace("É", "");
            valueToValidate = valueToValidate.Replace("ê", "");
            valueToValidate = valueToValidate.Replace("Ê", "");
            valueToValidate = valueToValidate.Replace("í", "");
            valueToValidate = valueToValidate.Replace("Í", "");
            valueToValidate = valueToValidate.Replace("ó", "");
            valueToValidate = valueToValidate.Replace("Ó", "");
            valueToValidate = valueToValidate.Replace("õ", "");
            valueToValidate = valueToValidate.Replace("Õ", "");
            valueToValidate = valueToValidate.Replace("ô", "");
            valueToValidate = valueToValidate.Replace("Ô", "");
            valueToValidate = valueToValidate.Replace("ú", "");
            valueToValidate = valueToValidate.Replace("Ú", "");
            valueToValidate = valueToValidate.Replace("ç", "");
            valueToValidate = valueToValidate.Replace("Ç", "");
            valueToValidate = valueToValidate.Replace(".", "");
            valueToValidate = valueToValidate.Replace(",", "");
            valueToValidate = valueToValidate.Replace("1", "");
            valueToValidate = valueToValidate.Replace("2", "");
            valueToValidate = valueToValidate.Replace("3", "");
            valueToValidate = valueToValidate.Replace("4", "");
            valueToValidate = valueToValidate.Replace("5", "");
            valueToValidate = valueToValidate.Replace("6", "");
            valueToValidate = valueToValidate.Replace("7", "");
            valueToValidate = valueToValidate.Replace("8", "");
            valueToValidate = valueToValidate.Replace("9", "");
            valueToValidate = valueToValidate.Replace("0", "");
            valueToValidate = valueToValidate.Trim();
            valueToValidate = valueToValidate.Replace("  ", " ");

            while (valueToValidate.IndexOf("  ") > 0)
                valueToValidate = valueToValidate.Replace("  ", " ");

            return true;
        }
    }
}