package com.model.utils;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;
import java.io.StreamCorruptedException;

import org.apache.commons.codec.binary.Base64;

import com.datamodels.DeviceInfo;

import android.app.ActivityManager;
import android.app.ActivityManager.RunningServiceInfo;
import android.content.Context;
import android.content.SharedPreferences;
import android.content.SharedPreferences.Editor;

public class StorageHelper {

	public static void setPathAddr(Context context, String path) {
		SharedPreferences sharedPreferences = context.getSharedPreferences(
				"multcast", Context.MODE_MULTI_PROCESS);
		Editor editor = sharedPreferences.edit();
		editor.putString("addr", path);
		editor.commit();
	}

	public static void setDeviceInfo(Context context, DeviceInfo di) {
		ByteArrayOutputStream baos = new ByteArrayOutputStream();
		ObjectOutputStream oos;
		try {
			oos = new ObjectOutputStream(baos);
			oos.writeObject(di);

		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		// 将Product对象放到OutputStream中

		SharedPreferences sharedPreferences = context.getSharedPreferences(
				"deviceInfoBase64", Context.MODE_MULTI_PROCESS);
		// 将Product对象转换成byte数组，并将其进行base64编码
		String productBase64 = new String(Base64.encodeBase64(baos
				.toByteArray()));
		SharedPreferences.Editor editor = sharedPreferences.edit();

		editor.putString("deviceInfo" + di.getDeviceType(), productBase64);
		editor.commit();

	}

	public static DeviceInfo getDeviceInfo(Context context, String deviceType) {

		SharedPreferences sharedPreferences = context.getSharedPreferences(
				"deviceInfoBase64", Context.MODE_MULTI_PROCESS);

		String deviceInfoBase64 = sharedPreferences.getString("deviceInfo"
				+ deviceType, "");

		if (deviceInfoBase64 != null && !deviceInfoBase64.equals("")) { // 对Base64格式的字符串进行解码
			byte[] base64Bytes = Base64.decodeBase64(deviceInfoBase64
					.getBytes());
			ByteArrayInputStream bais = new ByteArrayInputStream(base64Bytes);
			
			try {
				ObjectInputStream ois = new ObjectInputStream(bais);
				DeviceInfo di;
				try {
					di = (DeviceInfo) ois.readObject();
					return di;
				} catch (ClassNotFoundException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
			
			} catch (StreamCorruptedException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			} catch (IOException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
				

			
		}

		return null;

	}

	public static void setIsOneMoreTime(Context context, boolean isSecend) {
		SharedPreferences sharedPreferences = context.getSharedPreferences(
				"isAnotherTime", Context.MODE_MULTI_PROCESS);
		Editor editor = sharedPreferences.edit();
		editor.putBoolean("secend", isSecend);
		editor.commit();
	}

	public static String getMulticastAddr(Context context, String def) {
		String add = "";
		SharedPreferences sharedPreferences = context.getSharedPreferences(
				"multcast", Context.MODE_MULTI_PROCESS);
		add = sharedPreferences.getString("addr", def);
		return add;
	}

	public static boolean getIsOneMoreTime(Context context, boolean isSecend) {
		SharedPreferences sharedPreferences = context.getSharedPreferences(
				"isAnotherTime", Context.MODE_MULTI_PROCESS);
		return sharedPreferences.getBoolean("secend", isSecend);
	}

	public static boolean isServiceRunning(Context context) {
		ActivityManager manager = (ActivityManager) context
				.getSystemService(Context.ACTIVITY_SERVICE);
		for (RunningServiceInfo service : manager
				.getRunningServices(Integer.MAX_VALUE)) {
			if ("jld.com.jld.remotecontrol.client.MainService"
					.equals(service.service.getClassName())) {
				return true;
			}
		}
		return false;
	}

}
