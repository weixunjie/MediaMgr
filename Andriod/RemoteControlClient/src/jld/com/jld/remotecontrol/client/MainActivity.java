package jld.com.jld.remotecontrol.client;

import java.sql.Date;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Timer;
import java.util.TimerTask;

import com.datamodels.DeviceInfo;
import com.example.iot.iot;
import com.example.iot.iot_observable;
import com.example.iot.state_observable;
import com.model.utils.DeviceOperation;
import com.model.utils.StorageHelper;

import android.app.Activity;
import android.app.AlertDialog;
import android.app.Dialog;
import android.content.ContentResolver;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.graphics.Color;
import android.graphics.drawable.BitmapDrawable;
import android.media.AudioManager;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.AdapterView.OnItemSelectedListener;
import android.widget.ArrayAdapter;
import android.widget.BaseAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;
import android.os.Build;
import android.provider.Settings;

public class MainActivity extends Activity {

	public interface registerCall {
		public void reg_done(String device_id, int _port_number);
	}

	public static registerCall registerCallFromService;

	private String[] devicesStrings = null;

	private final Timer timer = new Timer();
	private TimerTask task;
	private EditText editText1;

	private String regDeviceId;

	private int regPortNumber;

	private int regCurrentPort = 0;

	private Context mContext;
	private boolean isRegisting = false;
	private RadioOnClick DialogOnClick = new RadioOnClick(0);

	private RadioSetOuputTypeOnClick DialogSetOuptTypeOnClick = new RadioSetOuputTypeOnClick(
			0);

	private ListAdapter adapter;

	class RadioSetOuputTypeOnClick implements DialogInterface.OnClickListener {
		private int index;

		public RadioSetOuputTypeOnClick(int index) {
			this.index = index;
		}

		public void setIndex(int index) {
			this.index = index;
		}

		public int getIndex() {
			return index;
		}

		public void onClick(DialogInterface dialog, int whichButton) {

			StorageHelper.setAuditOutputType(mContext,
					String.valueOf(whichButton));

			setAudioOutMode(String.valueOf(whichButton));

			setIndex(whichButton);

			dialog.dismiss();

		}
	}

	class RadioOnClick implements DialogInterface.OnClickListener {

		private int index;

		public RadioOnClick(int index) {
			this.index = index;
		}

		public void setIndex(int index) {
			this.index = index;
		}

		public int getIndex() {
			return index;
		}

		public void onClick(DialogInterface dialog, int whichButton) {

			if (whichButton > 0) {
				DeviceInfo di = new DeviceInfo();
				di.setDeviceId(regDeviceId);
				di.setPortNumber(String.valueOf(regCurrentPort));
				di.setDeviceType(String.valueOf(whichButton));

				if (whichButton == 1) {
					di.setAC(true);
				} else {
					di.setAC(false);
				}

				di.setDeviceDisplay(devicesStrings[whichButton]);
				StorageHelper.setDeviceInfo(mContext, di);

				// default close
				DeviceOperation.setStatus(di, false, "", "", false);

			}

			setIndex(whichButton);

			dialog.dismiss();

			loadDeviceListViewData();

			if (regCurrentPort < regPortNumber - 1) {
				regCurrentPort++;
				Message msg = new Message();
				msg.what = 1;
				mHandler.sendMessage(msg);
			}

		}
	}

	private void setAudioOutMode(String mode) {
		

		AudioManager mAudioManager = (AudioManager) getSystemService(AUDIO_SERVICE);

		ArrayList<String> mList = new ArrayList<String>();

		if (mode.equals("0")) {
			mList.add(AudioManager.AUDIO_NAME_CODEC);
		}
		if (mode.equals("1")) {
			mList.add(AudioManager.AUDIO_NAME_HDMI);
		}

		mAudioManager.setAudioDeviceActive(mList,
				AudioManager.AUDIO_OUTPUT_ACTIVE);

		// Settings.System.SOUND_EFFECTS_ENABLED
		// mList = mAudioManager
		// .getActiveAudioDevices(AudioManager.AUDIO_OUTPUT_ACTIVE);
	}

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_main);

		mContext = this;

		String audioOutType = StorageHelper.getAuditOutputType(this, "0");

		setAudioOutMode(audioOutType);

		final Button btnChooseAuditOutput = (Button) findViewById(R.id.btnChooseAuditOutput);

		btnChooseAuditOutput.setOnClickListener(new View.OnClickListener() {

			@Override
			public void onClick(View arg0) {
				// TODO Auto-generated method stub

				String[] outputTypes = new String[] { "AUDIO_CODEC", "AUDIO_HDMI" };

				String types = StorageHelper.getAuditOutputType(mContext, "0");

				AlertDialog ad = new AlertDialog.Builder(MainActivity.this)
						.setTitle("��Ƶ���ѡ��")
						.setSingleChoiceItems(outputTypes,
								Integer.valueOf(types),
								DialogSetOuptTypeOnClick).create();
				// areaListView=ad.getListView();
				ad.show();

			}
		});

		String url = StorageHelper.getMulticastAddr(this, "");

		if (url.equals("")) {
			showSettingDialog(this);
		} else {

			if (StorageHelper.isServiceRunning(this)) {
				Intent intent = new Intent();
				intent.setClass(this, MainService.class);
				this.stopService(intent);

			} else {

				Intent intent = new Intent();
				intent.setClass(this, MainService.class);
				startService(intent);
				// finish();
			}
		}

		devicesStrings = new String[] {
				mContext.getString(R.string.ac_none_math),
				mContext.getString(R.string.ac_display),
				mContext.getString(R.string.tv_display),
				mContext.getString(R.string.projector_display),
				mContext.getString(R.string.computer_display),
				mContext.getString(R.string.ligtht_display) };

		bindDeviceListView();

		loadDeviceListViewData();

		registerCallFromService = new registerCall() {

			@Override
			public void reg_done(String device_id, int _port_number) {

				regDeviceId = device_id;

				regPortNumber = _port_number;

				regCurrentPort = 0;

				Message msg = new Message();
				msg.what = 1;
				mHandler.sendMessage(msg);

			}

		};

		final Button btnRegUnReg = (Button) findViewById(R.id.btnRegUnReg);

		final Button btnOpenSel = (Button) findViewById(R.id.btnOpenSel);

		btnOpenSel.setOnClickListener(new View.OnClickListener() {

			@Override
			public void onClick(View arg0) {

				DeviceInfo diInfo = adapter.listValues
						.get(adapter.selectedPosition);

				DeviceOperation.setStatus(diInfo, true, "", "", false);
			}
		});

		final Button btnCloseSel = (Button) findViewById(R.id.btnCloseSel);

		btnCloseSel.setOnClickListener(new View.OnClickListener() {

			@Override
			public void onClick(View arg0) {

				DeviceInfo diInfo = adapter.listValues
						.get(adapter.selectedPosition);

				DeviceOperation.setStatus(diInfo, false, "", "", false);
			}
		});

		btnRegUnReg.setOnClickListener(new View.OnClickListener() {

			@Override
			public void onClick(View arg0) {
				isRegisting = !isRegisting;
				if (isRegisting) {

					// weixunjie
					// registerCallFromService.reg_done("we", 4);
					DeviceOperation.ioClass.register();
					btnRegUnReg.setText("���ע��");
					btnOpenSel.setEnabled(false);
					btnCloseSel.setEnabled(false);
				} else {
					DeviceOperation.ioClass.unregister();
					btnRegUnReg.setText("��ʼע��");
					btnOpenSel.setEnabled(true);
					btnCloseSel.setEnabled(true);
				}

			}
		});

	}

	public class ListAdapter extends BaseAdapter {

		private int selectedPosition = -1;// ѡ�е�λ��
		private ArrayList<DeviceInfo> listValues;
		private LayoutInflater inflater;
		private View view;

		public ListAdapter(ArrayList<DeviceInfo> values) {
			// TODO Auto-generated constructor stub
			listValues = values;
		}

		@Override
		public int getCount() {
			// TODO Auto-generated method stub
			return listValues.size();
		}

		@Override
		public Object getItem(int position) {
			// TODO Auto-generated method stub
			return listValues.get(position);
		}

		@Override
		public long getItemId(int position) {
			// TODO Auto-generated method stub
			return position;
		}

		public void setSelectedPosition(int position) {
			selectedPosition = position;
		}

		@Override
		public View getView(final int position, View convertView,
				ViewGroup parent) {

			if (convertView == null) {
				convertView = LayoutInflater.from(mContext).inflate(
						R.layout.lv_item, null);

			}

			TextView tvItemText = (TextView) convertView
					.findViewById(R.id.tbItemText);

			tvItemText.setText(listValues.get(position).getDeviceDisplay()
					+ "(�豸��ţ�"
					+ listValues.get(position).getDeviceId()
					+ ",�˿�:"
					+ String.valueOf((Integer.valueOf(listValues.get(position)
							.getPortNumber()) + 1)) + ")");

			if (selectedPosition == position) {

				convertView.setBackgroundColor(Color.parseColor("#1E90FF"));
			} else {

				convertView.setBackgroundColor(Color.TRANSPARENT);

			}

			return convertView;

		}
	};

	private Handler mHandler = new Handler() {

		public void handleMessage(Message msg) {
			switch (msg.what) {
			case 1:

				// regDeviceId = device_id;
				//
				// regPortNumber = _port_number;
				//
				// regCurrentPort = 1;
				//
				AlertDialog ad = new AlertDialog.Builder(MainActivity.this)
						.setTitle(
								regDeviceId + "(�˿�:"
										+ String.valueOf(regCurrentPort + 1)
										+ ")ע���У�ѡ���豸����")
						.setSingleChoiceItems(devicesStrings, 0, DialogOnClick)
						.create();
				// areaListView=ad.getListView();
				ad.show();

				break;
			}
		};
	};

	private void loadDeviceListViewData() {

		adapter.listValues.clear();
		for (int i = 1; i <= 5; i++) {
			DeviceInfo di = StorageHelper.getDeviceInfo(mContext,
					String.valueOf(i));

			if (di != null) {
				adapter.listValues.add(di);

			}
		}

		adapter.notifyDataSetChanged();

	}

	private void bindDeviceListView() {

		adapter = new ListAdapter(new ArrayList<DeviceInfo>());
		ListView listView = (ListView) findViewById(R.id.lvDeviceList);
		listView.setAdapter(adapter);
		listView.setOnItemClickListener(new AdapterView.OnItemClickListener() {
			@Override
			public void onItemClick(AdapterView<?> parent, View view,
					int position, long id) {
				adapter.setSelectedPosition(position);
				adapter.notifyDataSetInvalidated();
			}
		});

	}

	private void showSettingDialog(final Context mContext) {

		LayoutInflater factory = LayoutInflater.from(mContext);
		final View textEntryView = factory
				.inflate(R.layout.serversetting, null);

		EditText addrEdit = new EditText(mContext);

		addrEdit.setText(StorageHelper.getMulticastAddr(mContext, ""));

		AlertDialog dlg = new AlertDialog.Builder(mContext)

				.setTitle("��������ַ")
				.setView(textEntryView)
				.setPositiveButton("��", new DialogInterface.OnClickListener() {
					public void onClick(DialogInterface dialog, int whichButton) {
						EditText addrEdit = (EditText) textEntryView
								.findViewById(R.id.addr_edit);
						String addr = addrEdit.getText().toString();
						// EditText ports = (EditText)
						// textEntryView.findViewById(R.id.ports_edit);
						if (addr != null && !"".equals(addr)/*
															 * && ports != null
															 * && !"".equals
															 * (ports)
															 */) {

							StorageHelper.setPathAddr(mContext, addr);

							if (StorageHelper.isServiceRunning(mContext)) {
								// notifyServer(COMMAND_CLIENT_UPDATEADDR);

								Intent intent = new Intent();
								intent.setClass(mContext, MainService.class);
								mContext.stopService(intent);

							} else {

								Intent intent = new Intent();
								intent.setClass(mContext, MainService.class);
								startService(intent);
								// finish();
							}

							// Toast.makeText(
							// mContext,
							// mContext.getString(R.string.serversettingok),
							// Toast.LENGTH_LONG).show();
							// finish();
						} else {
							// Dialog mDialog = new
							// MyDialog(mContext,R.style.MyDialog);
							// mDialog.show();
						}
					}
				})
				.setNegativeButton("ȡ��", new DialogInterface.OnClickListener() {
					public void onClick(DialogInterface dialog, int whichButton) {

					}
				}).create();
		dlg.show();
	}

	Handler handler = new Handler() {
		@Override
		public void handleMessage(Message msg) {
			// TODO Auto-generated method stub
			// Ҫ��������

			SimpleDateFormat formatter = new SimpleDateFormat("HH:mm:ss");
			Date curDate = new Date(System.currentTimeMillis());// ��ȡ��ǰʱ��
			String str = formatter.format(curDate);

			// textView1.setText(str);
			super.handleMessage(msg);
		}
	};

	/**
	 * A placeholder fragment containing a simple view.
	 */

}
