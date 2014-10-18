package jld.com.jld.remotecontrol.client;

import java.io.BufferedReader;
import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.net.Inet4Address;
import java.net.InetAddress;
import java.net.NetworkInterface;
import java.net.SocketException;
import java.util.ArrayList;
import java.util.Enumeration;
import java.util.List;
import java.util.Timer;
import java.util.TimerTask;
import java.util.concurrent.ExecutionException;

import microsoft.aspnet.signalr.client.ConnectionState;
import microsoft.aspnet.signalr.client.Logger;
import microsoft.aspnet.signalr.client.NullLogger;
import microsoft.aspnet.signalr.client.Platform;
import microsoft.aspnet.signalr.client.SignalRFuture;
import microsoft.aspnet.signalr.client.StateChangedCallback;
import microsoft.aspnet.signalr.client.http.android.AndroidPlatformComponent;
import microsoft.aspnet.signalr.client.hubs.HubConnection;
import microsoft.aspnet.signalr.client.hubs.HubProxy;
import microsoft.aspnet.signalr.client.hubs.SubscriptionHandler1;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import com.datamodels.DeviceInfo;
import com.datamodels.DeviceOperationQueue;
import com.example.iot.iot;
import com.example.iot.iot_observable;
import com.example.iot.state_observable;
import com.model.utils.DeviceOperation;
import com.model.utils.StorageHelper;

import android.R;
import android.app.Service;
import android.content.ContentResolver;
import android.content.Context;
import android.content.Intent;
import android.content.OperationApplicationException;
import android.media.AudioManager;
import android.os.Handler;
import android.os.IBinder;
import android.os.Message;
import android.util.Log;
import android.widget.Toast;

public class MainService extends Service {

	private HubProxy hub = null;

	Context mContext = null;

	String url = "http://192.168.1.100:19671/signalr/hubs";

	public static final String COMMAND_PLAY = "multicastplayer_play";
	public static final String COMMAND_STOP = "multicastplayer_stop";

	public static final String COMMAND_QUIT = "multicastplayer_QUIT";

	public static final int MESSAGE_RESTARTCLIENT = 1;

	public static final int MESSAGE_REPLAY = 2;

	public static final int MESSAGE_RECONNECT = 3;

	public static final int COMMAND_MANUAL_OPEN = 221;

	public static final int COMMAND_REQUEST_STATE = 234;

	public static final int COMMAND_MANUAL_CLOSE = 222;

	public static final int REMOTE_CONTRL_SEND_STATUS = 231;

	private ContentResolver mContentResolver = null;
	private boolean mIsClientRunning;

	private ArrayList<DeviceInfo> getStateList = new ArrayList<DeviceInfo>();

	private ArrayList<DeviceOperationQueue> scheduleQueue = new ArrayList<DeviceOperationQueue>();

	private String lockSchduelQuee = "String To Lock";

	private String lockSchduelQueeVol = "String To Lock Volumn";

	private boolean isAcOpen = false;
	private iot iot_hw;

	public HubConnection conn;

	private String path;

	private String guidId;

	private state_observable state_callback;
	private boolean mIsConnected = false;

	private boolean mTestOpen = false;

	final String TAG = "RCS";
	private AudioManager mAudioManager = null;

	private double maxMusicAudio = 100;

	double maxAdb = 14;

	double lastAdbValue = -1;

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

	// public String execCommand(String cmd) {
	//
	// String result = "";
	// DataOutputStream dos = null;
	// DataInputStream dis = null;
	//
	// try {
	// Process p = Runtime.getRuntime().exec("su");
	// dos = new DataOutputStream(p.getOutputStream());
	// dis = new DataInputStream(p.getInputStream());
	// System.out.println(cmd);
	//
	// dos.writeBytes(cmd + "\n");
	// dos.flush();
	// dos.writeBytes("exit\n");
	// dos.flush();
	//
	// String line = null;
	// while ((line = dis.readLine()) != null) {
	// result += line;
	// }
	// p.waitFor();
	// } catch (Exception e) {
	// e.printStackTrace();
	// } finally {
	// if (dos != null) {
	// try {
	// dos.close();
	// } catch (IOException e) {
	// e.printStackTrace();
	// }
	// }
	// if (dis != null) {
	// try {
	// dis.close();
	// } catch (IOException e) {
	// e.printStackTrace();
	// }
	// }
	// }
	// Log.d(TAG, "->Get the shell result:" + result.toString());
	// return result;
	//
	// // Runtime runtime = Runtime.getRuntime();
	// // Process proc;
	// // try {
	// // proc = runtime.exec("su");
	// //
	// // DataOutputStream os = new DataOutputStream(proc.getOutputStream());
	// // os.writeBytes(command );
	// // os.flush();
	// //
	// // try {
	// // if (proc.waitFor() != 0) {
	// // System.err.println("exit value = " + proc.exitValue());
	// // }
	// // } catch (InterruptedException e) {
	// // // TODO Auto-generated catch block
	// // e.printStackTrace();
	// // }
	// // BufferedReader in = new BufferedReader(new InputStreamReader(
	// // proc.getInputStream()));
	// // StringBuffer stringBuffer = new StringBuffer();
	// // String line = null;
	// // while ((line = in.readLine()) != null) {
	// // stringBuffer.append(line + " ");
	// // }
	// // System.out.println(stringBuffer.toString());
	// //
	// //
	// // return stringBuffer.toString();
	// //
	// // } catch (IOException e1) {
	// // // TODO Auto-generated catch block
	// // e1.printStackTrace();
	// // }
	// //
	// // return "";
	//
	// }

	public String execCommand() {

		String result = "";
		DataOutputStream dos = null;
		DataInputStream dis = null;
		try {

			String cmd = "cat sys/devices/platform/switch-gpio.0/headset_data";
			Process p = Runtime.getRuntime().exec(cmd);
			// dos = new DataOutputStream(p.getOutputStream());
			dis = new DataInputStream(p.getInputStream());
			System.out.println(cmd);

			// dos.writeBytes(cmd + "\n");
			// dos.flush();
			// dos.writeBytes("exit\n");
			// dos.flush();

			String line = null;
			while ((line = dis.readLine()) != null) {
				result += line;
			}
			p.waitFor();
		} catch (Exception e) {
			e.printStackTrace();
		} finally {
			// if (dos != null) {
			// try {
			// //dos.close();
			// } catch (IOException e) {
			// e.printStackTrace();
			// }
			// }
			if (dis != null) {
				try {
					dis.close();
				} catch (IOException e) {
					e.printStackTrace();
				}
			}
		}
		Log.d("Wei", "Get The Shell result:" + result.toString());
		return result.toString();
	}

	private Timer mTimer;

	private void setTimerTask() {

		mTimer.schedule(new TimerTask() {
			@Override
			public void run() {
				Message message = new Message();
				message.what = 1;
				doActionHandler.sendMessage(message);
			}
		}, 0, 301/* 表示1000毫秒之後，每隔1000毫秒執行一次 */);
	}

	/**
	 * do some action
	 */
	private Handler doActionHandler = new Handler() {
		@Override
		public void handleMessage(Message msg) {
			super.handleMessage(msg);
			int msgId = msg.what;
			switch (msgId) {
			case 1:

				synchronized (lockSchduelQueeVol) {

					String result = execCommand();// execCommand("cat sys/devices/platform/switch-gpio.0/headset_data");

					int numberStartIndex = result.indexOf("=");

					int numberEndIndex = result.indexOf(",");

					if (numberStartIndex > 0 && numberEndIndex > 0) {

						String volumnValueAdc = result.substring(
								numberStartIndex + 1, numberEndIndex);

						double currentAdbValue = Double.valueOf(volumnValueAdc
								.replace(" ", "").trim());

						int currentVolToSet = (int) (((currentAdbValue * 10000) / maxAdb) / 10000 * maxMusicAudio);

						if (currentAdbValue >= maxAdb) {
							currentVolToSet = (int) maxMusicAudio;
						}

						if (currentAdbValue <= 0) {
							currentVolToSet = 0;
						}

						Log.d(TAG, "Set :" + currentVolToSet + "of"
								+ maxMusicAudio);

						if (lastAdbValue != -1) {
							if (Math.abs(currentAdbValue - lastAdbValue) > 1) {

								lastAdbValue = currentAdbValue;
								mAudioManager.setStreamVolume(
										AudioManager.STREAM_MUSIC,
										currentVolToSet,
										AudioManager.FLAG_SHOW_UI);

							}
						} else {
							mAudioManager.setStreamVolume(
									AudioManager.STREAM_MUSIC, currentVolToSet,
									AudioManager.FLAG_SHOW_UI);
							lastAdbValue = currentAdbValue;

						}

					}
				}

				break;
			default:
				break;
			}
		}
	};

	@Override
	public void onStart(Intent intent, int startId) {
		// TODO Auto-generated method stub
		super.onStart(intent, startId);

		if (conn != null) {

			try
			{
			conn.stop();

			}
			catch(Exception ex)
			{
				
			}
			// conn.Stop();
			conn = null;
		}

		Platform.loadPlatformComponent(new AndroidPlatformComponent());

		mAudioManager = (AudioManager) getSystemService(Context.AUDIO_SERVICE);

		maxMusicAudio = mAudioManager
				.getStreamMaxVolume(AudioManager.STREAM_MUSIC);

		mTimer = new Timer();
		// start timer task
		//setTimerTask();

		mContentResolver = this.getContentResolver();

		url = StorageHelper.getMulticastAddr(this,
				"http://192.168.1.100:19671/signalr/hubs");

		mContext = this;

		try {
			connectToSignalRServer();
		} catch (SocketException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}

		iot_observable observable = new iot_observable() {

			@Override
			public int device_registered_callback(String device_id,
					int _port_number) {

				MainActivity.registerCallFromService.reg_done(device_id,
						_port_number);
				return 0;

			}
		};

		state_callback = new state_observable() {

			@Override
			public int state_callback(String device_id, int port, int state) {

				Log.e("weixunjie", "state_callback   id=" + device_id
						+ "state=" + state + "port=" + port);

				boolean isAc = false;
				String foundType = "";
				for (int i = 1; i <= 5; i++) {
					DeviceInfo di = StorageHelper.getDeviceInfo(mContext,
							String.valueOf(i));
					if (di != null) {

						if (di.getDeviceId().equals(device_id)
								&& di.getPortNumber().equals(
										String.valueOf(port))) {

							foundType = di.getDeviceType();
							isAc = di.isAC();
							break;
						}
					}
				}

				if (!foundType.equals("")) {

					Log.e("weixunjie", "sending  devve type  =" + foundType
							+ "Port" + String.valueOf(port) + "-->state:"
							+ String.valueOf(state));

					if (isAc) {
						isAcOpen = state == 1;
					}

					sendDeviceStatusToServer(foundType, String.valueOf(port),
							String.valueOf(state));
				}

				synchronized (lockSchduelQuee) {

					Log.e("laowei", "scheduleQueue+before size:"
							+ scheduleQueue.size());

					if (scheduleQueue.size() > 0) {

						scheduleQueue.remove(0);

						if (scheduleQueue.size() > 0) {
							DeviceOperation.setStatus(scheduleQueue.get(0)
									.getDeviceInfo(), scheduleQueue.get(0)
									.isOpen(), scheduleQueue.get(0)
									.getDeviceInfo().getAcMode(), scheduleQueue
									.get(0).getDeviceInfo().getAcTempure(),
									scheduleQueue.get(0).isSetMode());
						}

						Log.e("laowei", "scheduleQueue+now size:"
								+ scheduleQueue.size());
					}
				}

				if (getStateList.size() > 0) {

					int lastIndex = getStateList.size() - 1;

					int staes = iot_hw.get_state(getStateList.get(lastIndex)
							.getDeviceId(), Integer.valueOf(getStateList.get(
							lastIndex).getPortNumber()));
					getStateList.remove(lastIndex);

				}

				return 0;

			}

		};

		iot_hw = new iot(observable, state_callback);

		// Do it weixunjie
		iot_hw.init();

		DeviceOperation.ioClass = iot_hw;

		DeviceOperation.regDevicdId = new ArrayList<String>();

		Log.i(TAG, "onStart");

	}

	private void sendDeviceStatusToServer(String deviceType, String port,
			String state) {
		List<String> args = new ArrayList<String>();

		String js = "";

		JSONArray jsa = new JSONArray();
		JSONObject jo = new JSONObject();
		try {
			jo.put("commandType", REMOTE_CONTRL_SEND_STATUS);
			jo.put("guidId", guidId);
			jo.put("deviceType", deviceType);
			jo.put("port", port);
			jo.put("state", state);
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
			try {
				hub.invoke("SendRemoteControlToMgrServer", js,
						conn.getConnectionId()).get();
			} catch (InterruptedException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			} catch (ExecutionException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}

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

						conn.stop();

						// conn.Stop();
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
			try {
				hub.invoke("SendRemoteControlToMgrServer", js,
						conn.getConnectionId()).get();
			} catch (InterruptedException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			} catch (ExecutionException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}

		}

	}

	private void connectToSignalRServer() throws SocketException {
		if (mIsConnected) {
			return;
		}

		if (!url.startsWith("http://")) {
			url = "http://" + url;
		}

		if (url.indexOf("signalr/hubs") < 0) {
			url = url + "/signalr/hubs";
		}

		// Change to the IP address and matching port of your SignalR server.
		// String host = "http://192.168.0.xxx:8080";
		// HubConnection connection = new HubConnection( host );
		// HubProxy hub = connection.createHubProxy( "MessageHub" );

		// public HubConnection(String url, String queryString, boolean
		// useDefaultUrl, Logger logger) {
		// super(getUrl(url, useDefaultUrl), queryString, logger);
		// }
		conn = new HubConnection(url,
				"clientType=REMOTECONTORLDEVICE&clientIdentify="
						+ getLocalIPAddress(), true, new NullLogger());

		conn.stateChanged(new StateChangedCallback() {

			@Override
			public void stateChanged(ConnectionState arg0, ConnectionState arg1) {

				if (arg1 == ConnectionState.Disconnected) {
					mIsConnected = false;
					Log.i(TAG, "connection disconnected");
					// conn.Start();
					myHandler.sendEmptyMessageDelayed(MESSAGE_RECONNECT, 5000);
				} else if (arg1 == ConnectionState.Reconnecting) {
					Log.i(TAG, "connection Reconnecting");
				} else if (arg1 == ConnectionState.Connected) {

					// /sync device status

					mIsConnected = true;

					Toast.makeText(mContext, "后台连接成功", Toast.LENGTH_SHORT)
							.show();
				}
			};
		});

		// + getLocalIPAddress(),false,) {
		// @Override
		// public void OnError(Exception exception) {
		// // myHandler.sendEmptyMessageDelayed(MESSAGE_RECONNECT, 5000);
		// }
		//
		// @Override
		// public void OnMessage(String message) {
		//
		// }
		//
		// @Override
		// public void OnStateChanged(StateBase oldState, StateBase newState) {
		//

		// }
		// }
		// };

		hub = conn.createHubProxy("MediaMgrHub");

		hub.on("sendRemoteControlToClient", new SubscriptionHandler1<String>() {
			@Override
			public void run(String status) {

				{

					try {
						JSONObject resultObject = null;

						resultObject = new JSONObject(status);

						String command = null;

						int commandType = Integer.parseInt(resultObject.get(
								"commandType").toString());

						guidId = resultObject.get("guidId").toString();

						Log.i(TAG, "commandType: " + commandType);

						if (commandType == COMMAND_MANUAL_OPEN) {

							processManuOper(resultObject, true);

						} else if (commandType == COMMAND_MANUAL_CLOSE) {

							processManuOper(resultObject, false);
							// stop play
						}

						else if (commandType == COMMAND_REQUEST_STATE) {

							for (int i = 1; i <= 5; i++) {
								DeviceInfo di = StorageHelper.getDeviceInfo(
										mContext, String.valueOf(i));

								if (di != null) {

									if (!DeviceOperation.regDevicdId
											.contains(di.getDeviceId())) {

										DeviceOperation.regDevicdId.add(di
												.getDeviceId());
										iot_hw.iot_register_device(di
												.getDeviceId());

									}

									getStateList.add(di);

									// sendDeviceStatusToServer(di.getDeviceType(),
									// String.valueOf(di.getPortNumber()),
									// String.valueOf(staes));

								}
							}

							if (getStateList.size() > 0) {

								int lastIndex = getStateList.size() - 1;

								int staes = iot_hw.get_state(
										getStateList.get(lastIndex)
												.getDeviceId(), Integer
												.valueOf(getStateList.get(
														lastIndex)
														.getPortNumber()));
								getStateList.remove(lastIndex);

							}

							// processManuOper(resultObject, false);
							// stop play
						}

						// notifyCommand(command, bean, mIsClientRunning);
						// hub.Invoke("sendPlayResponseMessage", args, null);

					} catch (JSONException e) {
						// TODO Auto-generated catch block
						e.printStackTrace();
					}
				}
			}
		}, String.class);

		// hub.On("sendRemoteControlToClient", new HubOnDataCallback() {
		// @Override
		// public void OnReceived(JSONArray args)
		//
		// });

		SignalRFuture<Void> awaitConnection = conn.start();
		try {
			awaitConnection.get();
		} catch (InterruptedException e) {
			// Handle ...
		} catch (ExecutionException e) {
			// Handle ...
		}
		// conn.Start();
	}

	private String getDeviceDisplayByType(String deviceType) {
		if (deviceType.equals(DeviceOperation.DEVICE_AC)) {
			return mContext
					.getString(jld.com.jld.remotecontrol.client.R.string.ac_display);
		} else if (deviceType.equals(DeviceOperation.DEVICE_TV)) {
			return mContext
					.getString(jld.com.jld.remotecontrol.client.R.string.tv_display);
		} else if (deviceType.equals(DeviceOperation.DEVICE_PROJECTOR)) {
			return mContext
					.getString(jld.com.jld.remotecontrol.client.R.string.projector_display);
		} else if (deviceType.equals(DeviceOperation.DEVICE_PC)) {
			return mContext
					.getString(jld.com.jld.remotecontrol.client.R.string.computer_display);
		} else if (deviceType.equals(DeviceOperation.DEVICE_LIGHT)) {
			return mContext
					.getString(jld.com.jld.remotecontrol.client.R.string.ligtht_display);
		}

		return "";
	}

	private void processManuOper(JSONObject resultObject, boolean isOpen) {
		JSONObject argObject;
		try {
			argObject = new JSONObject(resultObject.get("arg").toString());

			String devicesType = argObject.get("devicesType").toString();

			if (devicesType != null && !devicesType.equals("")) {

				String[] strs = devicesType.split(",");

				if (strs != null && strs.length > 0) {
					for (int i = 0; i < strs.length; i++) {

						String deviceType = strs[i];

						String operText = "打开";

						if (!isOpen) {
							operText = "关闭";
						}
						DeviceInfo di = StorageHelper.getDeviceInfo(mContext,
								deviceType);

						if (di != null) {

							JSONObject paramsDataObject = new JSONObject(
									argObject.get("paramsData").toString());

							String acTemperature = paramsDataObject.get(
									"acTemperature").toString();

							String acMode = paramsDataObject.get("acMode")
									.toString();

							boolean oResult = true;

							DeviceOperationQueue queu = new DeviceOperationQueue();
							di.setAcMode(acMode);

							di.setAcTempure(acTemperature);
							queu.setDeviceInfo(di);
							queu.setOpen(isOpen);

							synchronized (lockSchduelQuee) {

								Log.e("laowei",
										"scheduleQueue COMMAND before size:"
												+ scheduleQueue.size());
								if (scheduleQueue.size() <= 0) {

									scheduleQueue.add(queu);

									if (!acMode.equals("")) {

										if (isAcOpen) {
											DeviceOperation.setStatus(di,
													isOpen, acMode,
													acTemperature, true);
										} else {
											queu.setSetMode(true);
											scheduleQueue.add(queu);

											DeviceOperation.setStatus(di,
													isOpen, acMode,
													acTemperature, false);
										}
									}

									else {

										DeviceOperation.setStatus(di, isOpen,
												acMode, acTemperature, false);
									}

									// oResult = DeviceOperation.setStatus(di,
									// isOpen, acMode, acTemperature,
									// false);

									// if ()

								} else {
									scheduleQueue.add(queu);
								}

								Log.e("laowei",
										"scheduleQueue COMMAND afeter size:"
												+ scheduleQueue.size());
							}

							if (oResult) {
								sendFeedBackToServer("0",

								getDeviceDisplayByType(deviceType) + operText
										+ "成功");
							} else {
								sendFeedBackToServer("0",

								getDeviceDisplayByType(deviceType) + operText
										+ "失败");
							}

						} else {
							sendFeedBackToServer("21",

							getDeviceDisplayByType(deviceType) + "未注册");
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
