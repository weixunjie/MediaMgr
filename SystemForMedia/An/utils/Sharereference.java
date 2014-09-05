package com.model.utils;

import android.app.ActivityManager;
import android.app.ActivityManager.RunningServiceInfo;
import android.content.Context;
import android.content.SharedPreferences;
import android.content.SharedPreferences.Editor;

public class Sharereference {
	
	public static void setPathAddr (Context context, String path) {
		SharedPreferences sharedPreferences = context.getSharedPreferences("multcast",Context.MODE_MULTI_PROCESS);
		Editor editor= sharedPreferences.edit();
		editor.putString("addr",path);
		editor.commit();
	}
	
	public static void setIsOneMoreTime (Context context, boolean isSecend) {
		SharedPreferences sharedPreferences = context.getSharedPreferences("isAnotherTime",Context.MODE_MULTI_PROCESS);
		Editor editor= sharedPreferences.edit();
		editor.putBoolean("secend",isSecend);
		editor.commit();
	}
	
	public static String getMulticastAddr (Context context,String def) {
		String add = "";	
		SharedPreferences sharedPreferences = context.getSharedPreferences("multcast",Context.MODE_MULTI_PROCESS);
		add = sharedPreferences.getString("addr", def);
		return add;		
	}
	
	public static boolean getIsOneMoreTime (Context context, boolean isSecend) {
		SharedPreferences sharedPreferences = context.getSharedPreferences("isAnotherTime",Context.MODE_MULTI_PROCESS);
		return sharedPreferences.getBoolean("secend", isSecend);
	}
	
	public static boolean isServiceRunning(Context context) {
		ActivityManager manager = (ActivityManager) context
				.getSystemService(Context.ACTIVITY_SERVICE);
		for (RunningServiceInfo service : manager
				.getRunningServices(Integer.MAX_VALUE)) {
			if ("com.model.commandreceiver.CommandReceiver".equals(service.service.getClassName())) {
				return true;
			}
		}
		return false;
	} 

}
