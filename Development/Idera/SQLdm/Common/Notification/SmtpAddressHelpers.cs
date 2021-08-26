//------------------------------------------------------------------------------
// <copyright file="SmtpAddressHelpers.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Net.Mail;
using System.IO;

namespace Idera.SQLdm.Common.Notification
{
    public static class SmtpAddressHelpers
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("SmtpAddressHelpers");

        public static bool ParseAddressesInto(string addresses, MailAddressCollection addressCollection)
        {
            StringBuilder builder = new StringBuilder();
            string[] chunks = addresses.Split(new char[] { '@' });
            if (chunks.Length > 0)
                builder.Append(chunks[0].Trim());

            for (int i = 1; i < chunks.Length; i++)
            {
                try
                {
                    builder.Append("@");
                    int j = chunks[i].IndexOfAny(new char[] { ',', ';' });
                    if (j == -1)
                    {
                        if (i + 1 < chunks.Length)
                            throw new InvalidDataException();
                        builder.Append(chunks[i]);
                        addressCollection.Add(new MailAddress(builder.ToString()));
                        builder.Length = 0;
                        break;
                    }
                    else
                    {
                        builder.Append(chunks[i].Substring(0, j));
                        addressCollection.Add(new MailAddress(builder.ToString()));
                        builder.Length = 0;
                        builder.Append(chunks[i].Substring(j + 1));
                    }
                }
                catch (Exception)
                {
                    LOG.VerboseFormat("Error parsing SMTP To address: {0}", addresses);
                    return false;
                }
            }
            if (builder.Length > 0)
            {
                LOG.VerboseFormat("Error parsing SMTP To address: {0}", addresses);
                return false;
            }

            return true;
        }

        public static bool IsMailAddressValid(string value, bool single)
        {
            bool result = false;
            try
            {
                if (single)
                {
                    MailAddress address = new MailAddress(value);
                    result = true;
                }
                else
                {
                    MailAddressCollection mac = new MailAddressCollection();
                    result = SmtpAddressHelpers.ParseAddressesInto(value, mac);
                }
            }
            catch (Exception)
            {

            }
            return result;
        }

    }
}
