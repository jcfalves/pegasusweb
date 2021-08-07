using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bayer.Pegasus.Utils
{
    public class RegularExpressions
    {
        private const string sAcc = @"|ç|ã|ñ|õ|á|é|í|ó|ú|à|è|ì|ò|ù|ä|ë|ï|ö|ü|â|ê|î|ô|û|Ç|Ã|Ñ|Õ|Á|É|Í|Ó|Ú|À|È|Ì|Ò|Ù|Ä|Ë|Ï|Ö|Ü|Â|Ê|Î|Ô|Û|'";

        /****************************************/
        // Expressões Regulares - STRING

        /// <summary>
        /// Alfa
        /// </summary>
        public const string Alfa = "^[a-zA-Z]+$";

        public const string ValorDecimal = "numero";

        public const string Data = "data";

        public const string Inteiro = "data";

        /// <summary>
        /// Alfa e espaço
        /// </summary>
        public const string AlfaComEspaco = "^[a-zA-Z ]+$";

        /// <summary>
        /// Alfanumérico
        /// </summary>
        public const string AlfaNumerico = "^[a-zA-Z0-9]+$";

        /// <summary>
        /// Alfanumérico e espaço
        /// </summary>
        public const string AlfaNumericoComEspaco = "^[a-zA-Z0-9 ]+$";

        /// <summary>
        /// Alfanumérico e separadores (espaço, tab, quebra de linha)
        /// </summary>
        public const string AlfaNumericoComSeparadores = @"^[a-zA-Z0-9\s]+$";

        /// <summary>
        /// Alfa acentuado
        /// </summary>
        public const string AlfaAcentuado = "^[a-zA-Z" + sAcc + "]+$";

        /// <summary>
        /// Alfa e espaço acentuado
        /// </summary>
        public const string AlfaComEspacoAcentuado = "^[a-zA-Z " + sAcc + "]+$";

        /// <summary>
        /// Alfanumérico acentuado
        /// </summary>
        public const string AlfaNumericoAcentuado = "^[a-zA-Z0-9" + sAcc + "]+$";

        /// <summary>
        /// Alfanumérico e espaço
        /// a-z / A-Z / 0-9 / " " +acentuação
        /// </summary>
        public const string AlfaNumericoComEspacoAcentuado = "^[a-zA-Z0-9 " + sAcc + "]+$";

        /// <summary>
        /// Alfanumérico e separadores (espaço, tab, quebra de linha)
        /// </summary>
        public const string AlfaNumericoComSeparadoresAcentuado = @"^[a-zA-Z0-9\s" + sAcc + "]+$";

        /// <summary>
        /// Alfanumérico e separadores (espaço, tab, quebra de linha)
        /// </summary>
        public const string AlfaNumericoComSeparadoresAcentuadoPontuado = @"^[a-zA-Z0-9\s" + sAcc + @"|.|,|?|!|@|#|$|&|%|*" + "]+$";

        /// <summary>
        /// Word (alfanuméricos não acentuados, números e underscore)
        /// </summary>
        public const string AlfaNumericoSublinhado = @"^[\w]+$";

        /// <summary>
        /// Aceita caracteres, número, sublinhado e ponto
        /// </summary>
        public const string AlfaNumericoSublinhadoComPonto = @"^[\w.]+$";

        /// <summary>
        /// Ids
        /// </summary>
        public const string ID = @"^{*[\w|-]+}*$";

        /// <summary>
        /// String qualquer caractere é aceito
        /// </summary>
        public const string QualquerCaractere = @"^([^<]|<[^>]*$)*$";

        public const string Telefone = @"^\(?\d{2}\)?-? *\d{4,5}-? *-?\d{4}$";

        /* Expressões regulares para customização */

        /// <summary>
        /// CEP
        /// 99999-999
        /// </summary>
        public const string CEP = "^\\d{5}-\\d{3}$";

        /// <summary>
        /// RG
        /// (99)9.999.999-X
        /// </summary>
        public const string RG = "^\\d{1,3}.\\d{3}.\\d{3}-[a-zA-Z0-9]$";

        /// <summary>
        /// CPF
        /// 999.999.999-99 ou 99999999999
        /// </summary>
        public const string CPF = "^((\\d{3}.\\d{3}.\\d{3}-\\d{2})|(\\d{3}\\d{3}\\d{3}\\d{2}))$";

        /// <summary>
        /// CNPJ
        /// 99.999.999/9999-99 ou 99999999999999
        /// </summary>
        public const string CNPJ = "^((\\d{2}.\\d{3}.\\d{3}\\/\\d{4}-\\d{2})|(\\d{2}\\d{3}\\d{3}\\d{4}\\d{2}))$";

        /// <summary>
        ///	strPattern: nomeDotAtom@dominioInternet
        ///	nomeDotAtom 
        ///	- caracteres validos: a-z A-Z 0-9 ! # $ % & ' * + - / = ? ^ _ ` { | } ~
        ///	- O ponto (.) é usado como separador de palavras, e nao pode, portanto
        ///	  ser o primeiro ou o ultimo caracter do nome.
        ///	- fonte: RFC 2822 (3.4.1) http://www.rfc-editor.org/rfc/rfc2822.txt
        ///	dominioInternet 
        ///	- caracteres validos: a-z A-Z 0-9
        ///	- O hifen (-) é usado como separador de palavras, e nao pode, portanto
        ///   ser o primeiro ou o ultimo caracter do dominio.
        ///	- fontes: 
        ///	http://registro.br/info/dicas.html
        ///	RFC 1035 (2.3.1) http://www.rfc-editor.org/rfc/rfc1035.txt
        ///	glossário
        ///	\	: caracter de escape
        ///	\w	: é o mesmo que [a-zA-Z0-9_]
        ///	+	: uma ou mais vezes
        ///	*	: zero ou mais vezes
        ///	()	: agrupamento
        /// </summary>
        public const string Email = @"(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])";

        /// <summary>
        /// Ip
        /// </summary>
        public const string Ip = "^\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}$";

        /// <summary>
        /// Ip Faixa
        /// </summary>
        public const string IpFaixa = "^\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.(\\d{1,3}|\\*)$";

        /// <summary>
        /// Hora:Minuto
        /// </summary>
        public const string Hora = @"([01]\d|2[0-3]):[0-5]\d";

        /// <summary>
        /// Hora:Minuto:Segundo
        /// </summary>
        public const string HoraSegundo = @"([01]\d|2[0-3]):[0-5]\d:[0-5]\d";
    }
}
