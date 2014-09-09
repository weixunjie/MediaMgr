package jld.com.jld.remotecontrol.client;

import java.net.Inet4Address;
import java.net.InetAddress;
import java.net.NetworkInterface;
import java.net.SocketException;
import java.util.ArrayList;
import java.util.Enumeration;
import java.util.List;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import com.datamodels.DeviceInfo;
import com.model.utils.DeviceOperation;
import com.model.utils.StorageHelper;
import com.zsoft.SignalA.ConnectionState;
import com.zsoft.SignalA.Hubs.HubConnection;
import com.zsoft.SignalA.Hubs.HubInvokeCallback;
import com.zsoft.SignalA.Hubs.HubOnDataCallback;
import com.zsoft.SignalA.Hubs.IHubProxy;
import com.zsoft.SignalA.Transport.StateBase;
import com.zsoft.SignalA.Transport.Longpolling.ConnectedState;
import com.zsoft.SignalA.Transport.Longpolling.LongPollingTransport;

import android.app.Service;
import android.content.ContentResolver;
import android.content.Context;
import android.content.Intent;
import android.content.OperationApplicationException;
import android.os.Handler;
import android.os.IBinder;
import android.os.Message;
import android.util.Log;

public class MainService extends Service {

	private IHubProxy hub = null;

	Context mContext = null;

	String url = "http://192.168.1.100:19671/signalr/hubs";

	public static final String COMMAND_PLAY = "multicastplayer_play";
	public static final String COMMAND_STOP = "multicastplayer_stop";

	public static final String COMMAND_QUIT = "multicastplayer_QUIT";

	public static final int MESSAGE_RESTARTCLIENT = 1;

	public static final int MESSAGE_REPLAY = 2;

	public static final int MESSAGE_RECONNECT = 3;

	public static final int COMMAND_MANUAL_OPEN = 221;

	public static final int COMMAND_MANUAL_CLOSE = 222;

	

	private  ContentResolver mContentResolver = null;
	private boolean mIsClientRunning;

	private ConnectedState mState;

	public HubConnection conn;

	private String path;

	private String guidId;

	private boolean mIsConnected = false;

	private boolean mTestOpen = false;

	final String TAG = "Service";

	@Override
	public IBinder onBind(Intent intent) {
		// TODO Auto-generated method stub
		Log.i(TAG, "onBind");
		return null;
	}

	@Override
	public boolean onUnbind(Intent intent) {
		// TODO Auto-generated method stub
		Log.i(TAG, "onUnbind");
		return super.onUnbind(intent);
	}

	@Override
	public void onRebind(Intent intent) {
		// TODO Auto-generated method stub
		super.onRebind(intent);
		Log.i(TAG, "onRebind");
	}

	@Override
	public void onCreate() {
		// TODO Auto-generated method stub
		super.onCreate();
		Log.i(TAG, "onCreate");
	}

	@Override
	public void onDestroy() {
		// TODO Auto-generated method stub
		super.onDestroy();
		Log.i(TAG, "onDestroy");
	}

	@Override
	public void onStart(Intent intent, int startId) {
		// TODO Auto-generated method stub
		super.onStart(intent, startId);

		mContentResolver=this.getContentResolver();
		
		url = StorageHelper.getMulticastAddr(this,
				"http://192.168.1.100:19671/signalr/hubs");

		mContext = this;

		try {
			connectToSignalRServer();
		} catch (SocketException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		Log.i(TAG, "onStart");
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

	Handler myHandler = new Handler() {

		@Override
		public void handleMessage(Message msg) {
			// TODO Auto-generated method stub
			super.handleMessage(msg);

			switch (msg.what) {
			case MESSAGE_RESTARTCLIENT:
				// notifyCommand(COMMAND_PLAY, bean, true);
				mIsClientRunning = true;
				break;

			case MESSAGE_RECONNECT:

				try {

					if (conn != null) {
						conn.Stop();
						conn = null;
					}
					connectToSignalRServer();
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

	private void sendFeedBackToServer(String error, String desp) {

		List<String> args = new ArrayList<String>();

		String js = "";

		JSONArray jsa = new JSONArray();
		JSONObject jo = new JSONObject();
		try {
			jo.put("guidId", guidId);
			jo.put("errorCode", error);
			jo.put("message", desp);
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

		if (hub != null) {
			hub.Invoke("SendRemoteControlToMgrServer", args,
					new HubInvokeCallback() {

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
									" send result to server exception:"
											+ ex.toString());
						}
					});
		}

	}

	private void connectToSignalRServer() throws SocketException {
		if (mIsConnected) {
			return;
		}

		if (!url.startsWith("http://")) {
			url = "http://" + url;
		}

		if (url.indexOf("signalr/hubs")<0)
		{
			url=url+"/signalr/hubs";
		}
		
		conn = new HubConnection(url , this,
				new LongPollingTransport(),
				"clientType=REMOTECONTORLDEVICE&clientIdentify="
						+ getLocalIPAddress()) {
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
					// Toast.makeText(mContext, R.string.serversucess,
					// Toast.LENGTH_SHORT).show();
				}
			}
		};

		try {
			hub = conn.CreateHubProxy("MediaMgrHub");

		} catch (OperationApplicationException e) {
			e.printStackTrace();
		}

		hub.On("sendRemoteControlToClient", new HubOnDataCallback() {
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

					if (commandType == COMMAND_MANUAL_OPEN) {
						
						processManuallyPlay(resultObject,true);

					} else if (commandType == COMMAND_MANUAL_CLOSE) {
						
						processManuallyPlay(resultObject,false);
						// stop play
					}

					// notifyCommand(command, bean, mIsClientRunning);
					// hub.Invoke("sendPlayResponseMessage", args, null);

				} catch (JSONException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
			}

		});
		conn.Start();
	}

	private void processManuallyPlay(JSONObject resultObject, boolean isOpen) {
		JSONObject argObject;
		try {
			argObject = new JSONObject(resultObject.get("arg").toString());

			String devicesType = argObject.get("devicesType").toString();

			if (devicesType != null && !devicesType.equals("")) {

				String[] strs = devicesType.split(",");

				if (strs != null && strs.length > 0) {
					for (int i = 0; i < strs.length; i++) {

						String deviceType = strs[i];

						String operText="打开";
						
						if (!isOpen)
						{
							operText="关闭";
						}
						DeviceInfo di = StorageHelper.getDeviceInfo(mContext,
								deviceType);

						if (di != null) {

							JSONObject paramsDataObject = new JSONObject(argObject.get("paramsData").toString());
							
							String acTemperature = paramsDataObject.get(
									"acTemperature").toString();

							String acMode = paramsDataObject.get("acMode").toString();

							boolean oResult = DeviceOperation.setStatus(di, isOpen, acMode, acTemperature);

							if (oResult) {
								sendFeedBackToServer(
										"0",
										DeviceOperation
												.getDeviceDisplayByType(deviceType)
												+ operText+"成功");
							} else {
								sendFeedBackToServer(
										"0",
										DeviceOperation
												.getDeviceDisplayByType(deviceType)
												+ operText+"失败");
							}

						} else {
							sendFeedBackToServer(
									"21",
									DeviceOperation
											.getDeviceDisplayByType(deviceType)
											+ "未注册");
						}

					}
				}

			}
		} catch (JSONException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}

	}

}
