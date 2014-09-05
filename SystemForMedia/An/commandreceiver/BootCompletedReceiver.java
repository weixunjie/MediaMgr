package com.model.commandreceiver;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.util.Log;

public class BootCompletedReceiver extends BroadcastReceiver {
	public static final String TAG = "BroadcastReceiver";
	@Override
	public void onReceive(Context context, Intent intent) {
		if (intent.getAction().equals(Intent.ACTION_BOOT_COMPLETED)) {
            Log.i(TAG, "boot over start service");
			Intent newIntent = new Intent(context, CommandReceiver.class);
			context.startService(newIntent);
		}
	}
} 
