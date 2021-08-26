package com.idera.sqldm.helpers;

import javax.crypto.Cipher;
import javax.crypto.SecretKey;
import javax.crypto.spec.SecretKeySpec;
import javax.xml.bind.DatatypeConverter;

public final class EncryptionHelper {
	private static final SecretKey secretkey;
	
	static {
		StringBuffer b = new StringBuffer("SQLelements");
		while(b.length() < 24) b.append(" ");
		byte[] sizedKey = b.toString().getBytes();
		secretkey = new SecretKeySpec(sizedKey, "DESede");
	}
	
	public static String quickEncrypt(String plaintext) throws Exception 
	{
		try {
			if (plaintext == null || plaintext.length() == 0) return plaintext;

			byte[] input = plaintext.getBytes("UTF-16LE");		
			Cipher c1 = Cipher.getInstance("TripleDES/ECB/PKCS5Padding");
			c1.init(Cipher.ENCRYPT_MODE, secretkey);
			byte[] output = c1.doFinal(input);
			return DatatypeConverter.printBase64Binary(output);
		} 
		catch (Exception e)
		{
			throw new Exception("Encrypt failed.  Inspect internal exception for more details.", e);
		}
	}	
	
	public static String quickDecrypt(String encryptedText) throws Exception 
	{
		try {
			if (encryptedText == null || encryptedText.length() == 0) return encryptedText;
			
			byte[] input = DatatypeConverter.parseBase64Binary(encryptedText);
			Cipher c1 = Cipher.getInstance("TripleDES/ECB/PKCS5Padding");
			c1.init(Cipher.DECRYPT_MODE, secretkey);
			byte[] output = c1.doFinal(input);
			return new String(output, "UTF-16LE");
		} 
		catch (Exception e)
		{
			throw new Exception("Decrypt failed.  Inspect internal exception for more details.", e);
		}
	}
	
}
