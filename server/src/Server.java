
import java.io.File;
import java.io.FileFilter;
import java.io.FileOutputStream;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;
import java.util.Locale;

import javax.crypto.Cipher;
import javax.crypto.SecretKey;
import javax.crypto.SecretKeyFactory;
import javax.crypto.spec.DESKeySpec;
import javax.crypto.spec.IvParameterSpec;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import sun.misc.BASE64Decoder;

/**
 * Servlet implementation class Server
 */
public class Server extends HttpServlet {
	private static final long serialVersionUID = 1L;

	private static BASE64Decoder base64Decoder = null;
	private final static String key = "19821005";
	private static Cipher cipher = null;
	private static SecretKey secretKey=null;
	private static IvParameterSpec iv=null;

	/**
	 * @see HttpServlet#HttpServlet()
	 */
	public Server() {
		super();
		// TODO Auto-generated constructor stub
		base64Decoder = new BASE64Decoder();
		
		try {
			byte[] KeyData = key.getBytes("UTF-8");
			DESKeySpec KS = new DESKeySpec(KeyData);
			SecretKeyFactory keyFactory = SecretKeyFactory.getInstance("DES");
            secretKey = keyFactory.generateSecret(KS);
            iv = new IvParameterSpec(KeyData);
			cipher = Cipher.getInstance("DES/CBC/PKCS5Padding");
		} catch (Exception e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}

	private String[] getRealParams(String value) {
		try {
			byte[] encryptParam = base64Decoder.decodeBuffer(value);
			cipher.init(Cipher.DECRYPT_MODE, secretKey, iv);
			byte[] decryptParam = cipher.doFinal(encryptParam);
			String param = new String(decryptParam,"UTF-8");
			String[] params = param.split("\n");
			return params;
		} catch (Exception e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		return null;
	}
	

	/**
	 * @see HttpServlet#doGet(HttpServletRequest request, HttpServletResponse
	 *      response)
	 */
	protected void doGet(HttpServletRequest request,
			HttpServletResponse response) throws ServletException, IOException {
		// TODO Auto-generated method stub
		response.setCharacterEncoding("UTF-8");
		if (cipher == null) {
			
			response.getOutputStream().print("f\ncipher is null!");
			return;
		}
		String value = request.getParameter("value");
		String[] params = getRealParams(value);
		if (params == null) {
			response.getOutputStream().print("f\nparams is error!");
			return;
		}
		String result = null;
		if ("create".equals(params[0])) {
			result = executeCreate(params);
		} else if ("append".equals(params[0])) {
			result = executeAppend(params);
		} else if ("delete".equals(params[0])) {
			result = executeDelete(params);
		} else if ("list".equals(params[0])) {
			result = executeList(params);
		} else {
			response.getOutputStream().print("f\nparam[0] is error!");
			return;
		}

		if (result != null) {
			response.getOutputStream().write(result.getBytes("UTF-8"));
		}
	}

	private String executeList(String[] params) {
		// TODO Auto-generated method stub
		StringBuilder result = new StringBuilder();
		List<String> files = getAllRARFile();
		result.append("t\nok");
		for (String file : files) {
			result.append("\n");
			result.append(file);
		}
		return result.toString();
	}

	private List<String> getAllRARFile() {
		// TODO Auto-generated method stub
		String path = this.getServletContext().getRealPath("");

		File directory = new File(path);
		
		File[] files = directory.listFiles(new FileFilter() {
			@Override
			public boolean accept(File pathname) {
				// TODO Auto-generated method stub
				String name=pathname.getName().toUpperCase(Locale.CHINESE);
				if(name.endsWith(".RAR"))
				{
					return true;
				}
				return false;
			}
		});
		
		List<String> results=new ArrayList<String>();
		for (File file : files) {
			results.add(file.getName());
		}

		return results;
	}

	private String executeDelete(String[] params) {
		// TODO Auto-generated method stub
		// delete,name
		StringBuilder result = new StringBuilder();
		if (params.length != 2) {
			result.append("f\nparams length is error");
			return result.toString();
		}

		String path = this.getServletContext().getRealPath(params[1]);
		File file = new File(path);
		if (file.exists()) {
			file.delete();
		}
		result.append("t\nok");
		return result.toString();
	}

	private String executeAppend(String[] params) {
		// append,name,content,start,size
		StringBuilder result = new StringBuilder();
		if (params.length != 5) {
			result.append("f\nparams length is error");
			return result.toString();
		}

		String path = this.getServletContext().getRealPath(params[1]);
		File file = new File(path);
		if (!file.exists()) {
			result.append("f\nthis file is not exists");
			return result.toString();
		}

		int start = Integer.parseInt(params[3]);
		int size = Integer.parseInt(params[4]);

		if (file.length() != start) {
			result.append("f\nstart position is error");
			return result.toString();
		}

		FileOutputStream fos = null;
		try {
			fos = new FileOutputStream(file,true);
			if (size != 0) {
				byte[] data = base64Decoder.decodeBuffer(params[2]);
				if (data.length != size) {
					result.append("f\ncontext size is error");
					return result.toString();
				}
				fos.write(data);
			}
			result.append("t\nok");
			return result.toString();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			result.append("f\n" + e.getMessage());
			return result.toString();
		} finally {
			if (fos != null) {
				try {
					fos.close();
				} catch (IOException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
			}
		}
	}

	private String executeCreate(String[] params) {
		// TODO Auto-generated method stub
		// create,name
		StringBuilder result = new StringBuilder();
		if (params.length != 2) {
			result.append("f\nparams length is error");
			return result.toString();
		}

		String path = this.getServletContext().getRealPath(params[1]);
		File file = new File(path);
		if (file.exists()) {
			result.append("f\nthis file exists");
			return result.toString();
		}
		try {
			if (file.createNewFile()) {
				result.append("t\nok");
				return result.toString();
			}
			result.append("f\nfile create fail");
			return result.toString();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			result.append("f\n" + e.getMessage());
			return result.toString();
		}
	}

}
