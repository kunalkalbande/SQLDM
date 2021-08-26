using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Schema.ScriptDom;
using Microsoft.Data.Schema.ScriptDom.Sql;

namespace Idera.SQLdm.Common.Snapshots
{
    public class SqlParsingHelper
    {
        // This is not the same as GetSignatureHash.  Check which one you really need.
        public static string GetStatementHash(string stmt)
        {
            return
                Convert.ToBase64String(HashSQLStatement(stmt));
        }

        // Do NOT pass a signature into here.  Pass the statement that you need a signature hash for.
        public static string GetSignatureHash(string fullSQLStatement)
        {
            return
                Convert.ToBase64String(HashSQLStatement(GetSignature(fullSQLStatement, false)));
        }

        private static byte[] HashSQLStatement(string stmt)
        {
            string salt = "2Oy9DUJz4eTCmtgvjt9N4yaIJGnxgRrxqyVTsGZEfyLKuY0TpNOCvTWRvwkS4gWyAi1s34YQbefu3GA5MWSNjk6YjSriP0sbB2HY";
            byte[] bytes = Encoding.UTF8.GetBytes(stmt + salt + (!String.IsNullOrEmpty(stmt) ? stmt.Length : 1));

            var sha1 = SHA1.Create();
            byte[] hashBytes = sha1.ComputeHash(bytes);
            return hashBytes;
        }

        // This signature is intended for display, not for hash creation
        public static string GetReadableSignature(string sqlText)
        {
            return GetSignature(sqlText, true);
        }


        private static string GetSignature(string sqlText, bool readable)
        {
            if (sqlText == null || sqlText.Length <= 0)
            {
                return null;
            }

            TSql90Parser parser = new TSql90Parser(false);
            string _textNormalized = null;

            sqlText = sqlText.TrimEnd(Environment.NewLine.ToCharArray());

            if (string.IsNullOrEmpty(sqlText))
            {
                return null;
            }

            sqlText = sqlText.Trim();

            // Avoiding exception from parser to create signature
            sqlText = sqlText.Replace((char)0xD, ' ').Replace((char)0xA, ' ').Replace((char)0xFFFF, ' ');

            if (string.IsNullOrEmpty(sqlText))
            {
                return null;
            }

            StringBuilder sb = new StringBuilder(sqlText.Length);
            using (TextReader reader = new StringReader(sqlText))
            {
                IList<ParseError> errors = new List<ParseError>();
                IList<TSqlParserToken> tokens = parser.GetTokenStream(reader, errors);
                foreach (TSqlParserToken token in tokens)
                {
                    if (TSqlTokenType.EndOfFile != token.TokenType)
                    {
                        if (KeepToken(token, readable))
                        {
                            if (readable)
                            {
                                // Maintain capitalization if intended to be displayed
                                sb.Append(token.Text);
                            }
                            else
                            {
                                sb.Append(token.Text.ToUpper());    
                            }
                            
                        }
                        else if (readable)
                        {
                            // Maintain placeholders for missing values if intended to be displayed
                            sb.Append('#');
                        }
                    }
                }

                _textNormalized = sb.ToString();

            }
            return (_textNormalized);
        }

        private static bool KeepToken(TSqlParserToken token, bool readable)
        {
            switch (token.TokenType)
            {
                case (TSqlTokenType.AsciiStringLiteral):
                case (TSqlTokenType.UnicodeStringLiteral):
                case (TSqlTokenType.EndOfFile):
                case (TSqlTokenType.Integer):
                case (TSqlTokenType.Real):
                case (TSqlTokenType.Double):
                case (TSqlTokenType.Money):
                case (TSqlTokenType.HexLiteral):
                     {
                        return (false);
                    }
                case (TSqlTokenType.WhiteSpace):
                case (TSqlTokenType.MultilineComment):
                case (TSqlTokenType.SingleLineComment):
                    {
                        return (readable);
                    }
            }
            return (true);
        }

    }
}
