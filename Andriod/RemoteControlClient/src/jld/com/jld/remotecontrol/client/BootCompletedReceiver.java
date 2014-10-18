package jld.com.jld.remotecontrol.client;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.os.Handler;
import android.util.Log;

public class BootCompletedReceiver extends BroadcastReceiver {
	public static final String TAG = "RMS";

	@Override
	public void onReceive(Context context, Intent intent) {

		final Context ct = context;

		Log.i(TAG, "boot over start service");
		
		final Intent it = intent;
		new Handler().postDelayed(new Runnable() {
			public void run() {

				Log.i(TAG, "boot over start service after 6 secs");
				if (it.getAction().equals(Intent.ACTION_BOOT_COMPLETED)) {
					Log.i(TAG, "boot over start service");
					Intent newIntent = new Intent(ct, MainActivity.class);
					newIntent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);

					ct.startActivity(newIntent);

				}
				// execute the task
			}
		}, 6000);

	}
}
