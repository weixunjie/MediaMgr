package com.model.commandreceiver;

import java.io.File;
import java.io.FileDescriptor;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.net.Inet4Address;
import java.net.InetAddress;
import java.net.NetworkInterface;
import java.net.SocketException;
import java.sql.Timestamp;
import java.util.ArrayList;
import java.util.Enumeration;
import java.util.List;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import android.R.integer;
import android.hardware.SerialManager;
import android.hardware.SerialPort;

import com.model.multicastplayer.MulticastAudioPlayer;
import com.model.multicastplayer.MulticastVideoPlayer;
import com.model.multicastplayer.R;
import com.model.utils.Sharereference;
import com.zsoft.SignalA.ConnectionState;
import com.zsoft.SignalA.Hubs.HubConnection;
import com.zsoft.SignalA.Hubs.HubInvokeCallback;
import com.zsoft.SignalA.Hubs.HubOnDataCallback;
import com.zsoft.SignalA.Hubs.IHubProxy;
import com.zsoft.SignalA.Transport.StateBase;
import com.zsoft.SignalA.Transport.Longpolling.ConnectedState;
import com.zsoft.SignalA.Transport.Longpolling.LongPollingTransport;

import android.app.Service;
import android.content.BroadcastReceiver;
import android.content.ContentResolver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.OperationApplicationException;
import android.content.res.AssetFileDescriptor;
import android.content.res.AssetManager;
import android.net.wifi.WifiInfo;
import android.net.wifi.WifiManager;
import android.os.Handler;
import android.os.IBinder;
import android.os.Message;
import android.os.SystemClock;
import android.provider.Settings;
import android.text.format.Time;
import android.util.Log;
import android.widget.Toast;

public class CommandReceiver extends Service {

	public static final String TAG = "CommandReceiver";

	private IHubProxy hub = null;

	Context mContext = null;

	String url = "http://192.168.1.100:19671/signalr/hubs";

	public static final String COMMAND_PLAY = "multicastplayer_play";
	public static final String COMMAND_STOP = "multicastplayer_stop";

	public SerialManager mSerialManager;
	public SerialPort mSerialPort;

	public static final String COMMAND_QUIT = "multicastplayer_QUIT";

	public static final int MESSAGE_RESTARTCLIENT = 1;

	public static final int MESSAGE_REPLAY = 2;

	public static final int MESSAGE_RECONNECT = 3;

	public static final int COMMAND_SYNC_TIME = 130;

	public static final int COMMAND_SERVER_PLAY = 111;

	public static final int COMMAND_SERVER_PSTOP = 112;

	public static final int COMMAND_CHANGE_IPADDR = 128;

	public static final int COMMAND_RESTART_DEVICE = 124;

	public static final int COMMAND_SHUTDOWN_DEVICE = 125;

	public static final int COMMAND_OPEN_SCREEN = 122;

	public static final int COMMAND_CLOSE_SCREEN = 123;

	public static final int COMMAND_SCHEDULE_TRUNONSHUTDOWN_DEVICE = 127;

	private boolean mIsClientRunning;

	private ConnectedState mState;

	public HubConnection conn;

	private String path;

	private String guidId;

	private boolean mIsConnected = false;

	private boolean mTestOpen = false;

	CommandBean bean = null;

	private ContentResolver mContentResolver = null;

	public MulticastAudioPlayer mAudioPlayer = null;

	@Override
	public IBinder onBind(Intent intent) {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public void onCreate() {
		// TODO Auto-generated method stub
		super.onCreate();

		mSerialManager = (SerialManager) getSystemService("serial");

		mContext = this;
		IntentFilter filter = new IntentFilter();
		filter.addAction(MulticastVideoPlayer.COMMAND_NETWORK_ERROR);
		filter.addAction(MulticastVideoPlayer.COMMAND_PLAYER_ERROR);
		filter.addAction(MulticastVideoPlayer.COMMAND_CLIENT_UPDATEADDR);
		filter.addAction(MulticastVideoPlayer.COMMAND_PLAY_RESULT);
		registerReceiver(mClientListener, filter);
		mIsClientRunning = false;
		url = Sharereference.getMulticastAddr(this,
				"http://192.168.1.100:19671/signalr/hubs");

		// play a short music when service start
		AssetManager assetManager = getAssets();
		AssetFileDescriptor in;
		try {
			Log.d("asserts test", "start to get asserts");
			// in = assetManager.openFd("dylan.mp3");
			in = assetManager.openFd("nosense.mp3");
			mAudioPlayer = new MulticastAudioPlayer(null);
			mAudioPlayer.play(in.getFileDescriptor());

			myHandler.postDelayed(new Runnable() {

				@Override
				public void run() {
					// TODO Auto-generated method stub
					if (mAudioPlayer != null) {
						mAudioPlayer.stop();
					}
				}
			}, 10000);

		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}

	}

	@Override
	public void onStart(Intent intent, int startId) {
		// TODO Auto-generated method stub
		super.onStart(intent, startId);
		try {
			mContentResolver = getContentResolver();
			beginConnect();
		} catch (SocketException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}

	@Override
	public void onDestroy() {
		// TODO Auto-generated method stub
		super.onDestroy();

		unregisterReceiver(mClientListener);

	}

	private BroadcastReceiver mClientListener = new BroadcastReceiver() {
		@Override
		public void onReceive(Context context, Intent intent) {

			String action = intent.getAction();
			Log.i(TAG, "onReceive action :" + action);

			if (action.equals(MulticastVideoPlayer.COMMAND_PLAYER_ERROR)) {
				myHandler.sendEmptyMessageDelayed(MESSAGE_RESTARTCLIENT, 1000);
			} else if (action.equals(MulticastVideoPlayer.COMMAND_CLIENT_QUIT)) {
				mIsClientRunning = false;
			} else if (action
					.equals(MulticastVideoPlayer.COMMAND_CLIENT_UPDATEADDR)) {
				url = Sharereference.getMulticastAddr(context,
						"http://192.168.1.100:19671/signalr/hubs");
			} else if (action
					.equals(MulticastVideoPlayer.COMMAND_OPEN_TEST_TAG)) {
				mTestOpen = true;
			} else if (action.equals(MulticastVideoPlayer.COMMAND_PLAY_RESULT)) {
				String errCode = intent.getStringExtra("errcode");
				serverCallBack(hub, errCode, "");
			}
		}
	};

	Handler myHandler = new Handler() {

		@Override
		public void handleMessage(Message msg) {
			// TODO Auto-generated method stub
			super.handleMessage(msg);

			switch (msg.what) {
			case MESSAGE_RESTARTCLIENT:
				notifyCommand(COMMAND_PLAY, bean, true);
				mIsClientRunning = true;
				break;

			case MESSAGE_RECONNECT:

				try {

					if (conn != null) {
						conn.Stop();
						conn = null;
					}
					beginConnect();
				} catch (SocketException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
				// conn.Start();
				break;

			default:

			}
		}

	};

	private void beginConnect() throws SocketException {
		if (mIsConnected) {
			return;
		}

		if (!url.startsWith("http://")) {
			url = "http://" + url;
		}

		if (url.indexOf("signalr/hubs") < 0) {
			url = url + "/signalr/hubs";
		}

		conn = new HubConnection(url, this, new LongPollingTransport(),
				"clientType=ANDROID&clientIdentify=" + this.getLocalIPAddress()) {
			@Override
			public void OnError(Exception exception) {
				// myHandler.sendEmptyMessageDelayed(MESSAGE_RECONNECT, 5000);
			}

			@Override
			public void OnMessage(String message) {

			}

			@Override
			public void OnStateChanged(StateBase oldState, StateBase newState) {

				if (newState.getState() == ConnectionState.Disconnected) {
					mIsConnected = false;
					Log.i(TAG, "connection disconnected");
					// conn.Start();
					myHandler.sendEmptyMessageDelayed(MESSAGE_RECONNECT, 5000);
				} else if (newState.getState() == ConnectionState.Reconnecting) {
					Log.i(TAG, "connection Reconnecting");
				} else if (newState.getState() == ConnectionState.Connected) {
					mIsConnected = true;
					Toast.makeText(mContext, R.string.serversucess,
							Toast.LENGTH_SHORT).show();
				}
			}
		};

		try {
			hub = conn.CreateHubProxy("MediaMgrHub");

		} catch (OperationApplicationException e) {
			e.printStackTrace();
		}

		hub.On("sendMessageToClient", new HubOnDataCallback() {
			@Override
			public void OnReceived(JSONArray args) {
				try {
					JSONObject resultObject = null;

					for (int i = 0; i < args.length(); i++) {
						resultObject = new JSONObject(args.opt(i).toString());
					}

					String command = null;

					int commandType = Integer.parseInt(resultObject.get(
							"commandType").toString());

					guidId = resultObject.get("guidId").toString();

					Log.i(TAG, "commandType: " + commandType);

					if (commandType == COMMAND_SERVER_PLAY) {

						JSONObject argObject = new JSONObject(resultObject.get(
								"arg").toString());
						String url = argObject.get("udpBroadcastAddress")
								.toString();
						String streamName = (String) argObject
								.get("streamName").toString();

						String mineType = argObject.get("mediaType").toString();
						String bitrate = argObject.get("bitRate").toString();

						String mediaType;
						if (mineType.equals("1")) {
							mediaType = "audio";
						} else {
							mediaType = "video";
						}

						if (bean == null) {
							bean = new CommandBean(url, mediaType, streamName,
									bitrate);
						} else {
							bean.mUdpBroadcastAddress = url;
							bean.mMediaType = mediaType;
							bean.mStreamName = streamName;
							bean.mBitrate = bitrate;
						}

						// start play
						command = COMMAND_PLAY;

						mIsClientRunning = true;

					} else if (commandType == COMMAND_SERVER_PSTOP) {
						command = COMMAND_STOP;
						mIsClientRunning = false;
						// stop play
					} else if (commandType == COMMAND_CHANGE_IPADDR) {

						JSONObject argObject = new JSONObject(resultObject.get(
								"arg").toString());
						String newIpAddress = argObject.get("newIpAddress")
								.toString();

						Settings.System.putInt(mContentResolver,
								Settings.System.WIFI_USE_STATIC_IP, 1);
						Settings.System.putString(mContentResolver,
								Settings.System.WIFI_STATIC_IP, newIpAddress);

						String serverUrl = argObject.get("serverUrl")
								.toString();

						if (serverUrl != null && !serverUrl.equals("")) {

							Sharereference.setPathAddr(mContext, serverUrl);
						}

						serverCallBack(hub, "0", "");
						// restart when change server url
						try {
							Thread.sleep(2000);
						} catch (InterruptedException e) {
							// TODO Auto-generated catch block
							e.printStackTrace();
						}

						try {
							mSerialManager.restart_system();
						} catch (Exception ex) {
						}

					}

					else if (commandType == COMMAND_RESTART_DEVICE) {

						serverCallBack(hub, "0", "");

						try {
							Thread.sleep(2000);
						} catch (InterruptedException e) {
							// TODO Auto-generated catch block
							e.printStackTrace();
						}

						try {
							mSerialManager.restart_system();
						} catch (Exception ex) {
						}

					}

					else if (commandType == COMMAND_SHUTDOWN_DEVICE) {

						serverCallBack(hub, "0", "");

						try {
							Thread.sleep(2000);
						} catch (InterruptedException e) {
							// TODO Auto-generated catch block
							e.printStackTrace();
						}

						try {
							mSerialManager.shutdown_system();
						} catch (Exception ex) {

						}
					}

					else if (commandType == COMMAND_OPEN_SCREEN) {

						serverCallBack(hub, "0", "");

						// mSerialManager.s()

						// mSerialManager.s
						// try {
						// Thread.sleep(2000);
						// } catch (InterruptedException e) {
						// // TODO Auto-generated catch block
						// e.printStackTrace();
						// }
						//
						// mSerialManager.save_system_time_to_rtc(millionsecond);

					}

					else if (commandType == COMMAND_CLOSE_SCREEN) {

						serverCallBack(hub, "0", "");

						// try {
						// Thread.sleep(2000);
						// } catch (InterruptedException e) {
						// // TODO Auto-generated catch block
						// e.printStackTrace();
						// }
						//
						// mSerialManager.save_system_time_to_rtc(millionsecond);

					}

					else if (commandType == COMMAND_SCHEDULE_TRUNONSHUTDOWN_DEVICE) {

						JSONObject argObject = new JSONObject(resultObject.get(
								"arg").toString());
						String scheduleTurnOnTime = argObject.get(
								"scheduleTurnOnTime").toString();

						String scheduleShutDownTime = argObject.get(
								"scheduleShutDownTime").toString();

						String isEnabled = argObject.get("isEnabled")
								.toString();

						String[] scheduleTurnOnTimeArray = scheduleTurnOnTime
								.split(":");

						String[] scheduleShutDownTimeArray = scheduleShutDownTime
								.split(":");

						int open_hour = Integer
								.valueOf(scheduleTurnOnTimeArray[0]);
						int open_minute = Integer
								.valueOf(scheduleTurnOnTimeArray[1]);

						int close_hour = Integer
								.valueOf(scheduleShutDownTimeArray[0]);
						int close_minute = Integer
								.valueOf(scheduleShutDownTimeArray[1]);

						try

						{
							int[] weeks = new int[] { 0x01, 0x02, 0x03, 0x04,
									0x05, 0x06, 0x07 };

							for (int i = 0; i < 7; i++) {
								int set_value[] = { weeks[i], open_hour,
										open_minute, close_hour, close_minute };
								mSerialManager.set_onoff_by_week(set_value);
							}

							int[] week_value = { 0, 0, 0, 0, 0, 0, 0 };

							week_value[0] = isEnabled.charAt(0) - 0x30;
							week_value[1] = isEnabled.charAt(0) - 0x30;
							week_value[2] = isEnabled.charAt(0) - 0x30;
							week_value[3] = isEnabled.charAt(0) - 0x30;
							week_value[4] = isEnabled.charAt(0) - 0x30;
							week_value[5] = isEnabled.charAt(0) - 0x30;
							week_value[6] = isEnabled.charAt(0) - 0x30;

							// mSerialManager.

							mSerialManager.enable_onoff_by_week(week_value);
						} finally {
						}

						serverCallBack(hub, "0", "");

					} else if (commandType == COMMAND_SYNC_TIME) {

						JSONObject argObject = new JSONObject(resultObject.get(
								"arg").toString());
						String serverNowTime = argObject.get("serverNowTime")
								.toString();

						try {
							SystemClock.setCurrentTimeMillis(Long
									.valueOf(serverNowTime));

						} catch (Exception ex) {

							Log.v(TAG, "Set Time Error" + ex.getMessage());
							// serverCallBack(hub,
							// "13","Set Time Error: "+ex.getMessage());
						}

					}
					notifyCommand(command, bean, mIsClientRunning);
					// hub.Invoke("sendPlayResponseMessage", args, null);

				} catch (JSONException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
			}

		});
		conn.Start();
	}

	MulticastAudioPlayer ap;

	private void notifyCommand(String command, CommandBean bean,
			boolean firstStart) {

		if (bean == null) {
			return;
		}

		Log.i(TAG, "notifyCommand:" + command + ", isfirststart:" + firstStart);

		if ("audio".equals(bean.mMediaType) && !mTestOpen) {
			if (command.equals(COMMAND_PLAY)) {
				if (ap != null) {
					ap.stop();
					ap = null;
				}
				ap = new MulticastAudioPlayer(this);

				if (bean.mMediaType == null || "".equals(bean.mMediaType)) {
					bean.mMediaType = "audio";
				}

				if (bean.mBitrate == null || "".equals(bean.mBitrate)
						|| Integer.parseInt(bean.mBitrate) <= 0) {
					bean.mBitrate = "320";
				}

				ap.play(bean.mUdpBroadcastAddress + "-" + bean.mMediaType + "/"
						+ bean.mBitrate);
			} else if (command.equals(COMMAND_STOP)) {
				if (ap != null) {
					ap.stop();
					ap = null;
				}
			}
			return;
		}

		if (bean.mMediaType == null || "".equals(bean.mMediaType)) {
			bean.mMediaType = "video";
		}

		if (bean.mBitrate == null || "".equals(bean.mBitrate)
				|| Integer.parseInt(bean.mBitrate) <= 0) {
			bean.mBitrate = "2000";
		}

		if (firstStart) {
			Intent intent = new Intent();
			intent.setClass(mContext, MulticastVideoPlayer.class);
			intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
			intent.putExtra("address", bean.mUdpBroadcastAddress + "-"
					+ bean.mMediaType + "/" + bean.mBitrate);
			intent.putExtra("mediaType", bean.mMediaType);
			intent.putExtra("streamName", bean.mStreamName);
			intent.putExtra("bitRate", bean.mBitrate);
			startActivity(intent);
			return;
		}

		if (command != null) {
			Intent i = new Intent();
			i.setAction(command);

			if (bean == null) {
				Log.e(TAG,
						"there is no server and stream info, mustbe some error happened!");
				return;
			}
			i.putExtra("address", bean.mUdpBroadcastAddress);
			i.putExtra("mediaType", bean.mMediaType);
			i.putExtra("streamName", bean.mStreamName);
			i.putExtra("bitrate", bean.mBitrate);
			sendBroadcast(i);
		}
	}

	private String getLocalIPAddress() throws SocketException {
		for (Enumeration<NetworkInterface> en = NetworkInterface
				.getNetworkInterfaces(); en.hasMoreElements();) {
			NetworkInterface intf = en.nextElement();
			for (Enumeration<InetAddress> enumIpAddr = intf.getInetAddresses(); enumIpAddr
					.hasMoreElements();) {
				InetAddress inetAddress = enumIpAddr.nextElement();
				if (!inetAddress.isLoopbackAddress()
						&& (inetAddress instanceof Inet4Address)) {
					return inetAddress.getHostAddress().toString();
				}
			}
		}
		return "null";
	}

	public String getIp() {

		WifiManager wifiManager = (WifiManager) getSystemService(Context.WIFI_SERVICE);

		if (!wifiManager.isWifiEnabled()) {
			wifiManager.setWifiEnabled(true);
		}
		WifiInfo wifiInfo = wifiManager.getConnectionInfo();
		int ipAddress = wifiInfo.getIpAddress();
		String ip = intToIp(ipAddress);
		return ip;
	}

	private String intToIp(int i) {

		return (i & 0xFF) + "." + ((i >> 8) & 0xFF) + "." + ((i >> 16) & 0xFF)
				+ "." + (i >> 24 & 0xFF);
	}

	private void serverCallBack(IHubProxy hub, String error, String message) {

		String errorCode = error;

		if (message.equals("")) {
			message = "Android Client Error";
		}

		List<String> args = new ArrayList<String>();

		String js = "";

		JSONArray jsa = new JSONArray();
		JSONObject jo = new JSONObject();
		try {
			jo.put("guidId", guidId);
			jo.put("errorCode", error);
			jo.put("message", message);
			jsa.put(0, jo);
			js = jsa.optString(0);
		} catch (JSONException e1) {
			// TODO Auto-generated catch block
			e1.printStackTrace();
		}

		args.add(js);
		if (conn != null) {
			args.add(conn.getConnectionId());
		}
		// args.add(errorCode);
		// args.add(message);
		// try {
		// args.add(getLocalIPAddress());
		// } catch (SocketException e) {
		// // TODO Auto-generated catch block
		// e.printStackTrace();
		// }

		if (hub != null) {
			hub.Invoke("SendMessageToMgrServer", args, new HubInvokeCallback() {

				@Override
				public void OnResult(boolean succeeded, String response) {
					// TODO Auto-generated method stub
					Log.d(TAG, " send result to server"
							+ (succeeded ? "success" : "failed"));
				}

				@Override
				public void OnError(Exception ex) {
					// TODO Auto-generated method stub
					Log.d(TAG,
							" send result to server exception:" + ex.toString());
				}
			});
		}

	}
}
