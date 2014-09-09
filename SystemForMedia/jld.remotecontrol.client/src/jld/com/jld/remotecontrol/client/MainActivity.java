package jld.com.jld.remotecontrol.client;

import java.sql.Date;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Timer;
import java.util.TimerTask;

import com.datamodels.DeviceInfo;
import com.example.iot.iot;
import com.example.iot.iot_observable;
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

public class MainActivity extends Activity {

	public interface huidiaoCallback {
		public void hui(String str);
	}

	private String[] devicesStrings = new String[] {
			this.getString(R.string.ac_display),
			this.getString(R.string.tv_display),
			this.getString(R.string.projector_display),
			this.getString(R.string.computer_display),
			this.getString(R.string.ligtht_display) };

	private iot iot_hw;

	private final Timer timer = new Timer();
	private TimerTask task;
	private EditText editText1;

	private String regDeviceId;

	private String regPortNumber;

	private Context mContext;
	private boolean isRegisting = false;
	private RadioOnClick DialogOnClick = new RadioOnClick(0);

	private ListAdapter adapter;

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

			DeviceInfo di = new DeviceInfo();
			di.setDeviceId(regDeviceId);
			di.setPortNumber(regPortNumber);
			di.setDeviceType(String.valueOf(whichButton + 1));

			if (whichButton == 0) {
				di.setAC(true);
			} else {
				di.setAC(false);
			}

			di.setDeviceDisplay(devicesStrings[whichButton]);
			StorageHelper.setDeviceInfo(mContext, di);
			setIndex(whichButton);

			dialog.dismiss();

			loadDeviceListViewData();
		}
	}

	public class ListAdapter extends BaseAdapter {

		private int selectedPosition = -1;// 选中的位置
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
					+ "(设备编号：" + listValues.get(position).getDeviceId() + ")");

			if (selectedPosition == position) {

				convertView.setBackgroundColor(Color.parseColor("#1E90FF"));
			} else {

				convertView.setBackgroundColor(Color.TRANSPARENT);

			}

			return convertView;

		}
	};

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_main);

		task = new TimerTask() {
			@Override
			public void run() {
				// TODO Auto-generated method stub
				Message message = new Message();
				message.what = 1;
				handler.sendMessage(message);
			}
		};

		// editText1 = (EditText) findViewById(R.id.editText1);

		bindDeviceListView();

		loadDeviceListViewData();

		// io.register();

		// io.

		// huidiao(new huidiaoCallback() {
		//
		// @Override
		// public void hui(String str) {
		//
		// AlertDialog ad = new AlertDialog.Builder(MainActivity.this)
		// .setTitle(str + "已经注册中，选择设备类型")
		// .setSingleChoiceItems(devicesStrings, 0, DialogOnClick)
		// .create();
		// // areaListView=ad.getListView();
		// ad.show();
		// }
		// });

		iot_observable observable = new iot_observable() {

			@Override
			public int device_registered_callback(String device_id,
					int _port_number) {

				regDeviceId = device_id;
				regPortNumber = String.valueOf(_port_number);
				Log.d("MainActivity", "deivice=" + device_id + "  port="
						+ _port_number + "   registed");

				AlertDialog ad = new AlertDialog.Builder(MainActivity.this)
						.setTitle(device_id + "已经注册中，选择设备类型")
						.setSingleChoiceItems(devicesStrings, 0, DialogOnClick)
						.create();
				// areaListView=ad.getListView();
				ad.show();

				return 0;

			}
		};

		iot_hw = new iot(observable);
		iot_hw.init();

		DeviceOperation.ioClass = iot_hw;
		final Button btnRegUnReg = (Button) findViewById(R.id.btnRegUnReg);

		btnRegUnReg.setOnClickListener(new View.OnClickListener() {

			@Override
			public void onClick(View arg0) {
				isRegisting = !isRegisting;
				if (isRegisting) {
					iot_hw.register();
					btnRegUnReg.setText("完成注册");
				} else {
					iot_hw.unregister();
					btnRegUnReg.setText("开始注册");
				}

			}
		});

		final Button btnOpenSel = (Button) findViewById(R.id.btnOpenSel);

		btnOpenSel.setOnClickListener(new View.OnClickListener() {

			@Override
			public void onClick(View arg0) {

				DeviceInfo diInfo = adapter.listValues
						.get(adapter.selectedPosition);

				DeviceOperation.setStatus(diInfo, true, "", "");
			}
		});

		final Button btnCloseSel = (Button) findViewById(R.id.btnCloseSel);

		btnCloseSel.setOnClickListener(new View.OnClickListener() {

			@Override
			public void onClick(View arg0) {

				DeviceInfo diInfo = adapter.listValues
						.get(adapter.selectedPosition);

				DeviceOperation.setStatus(diInfo, false, "", "");
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

		// btn.setOnClickListener(new View.OnClickListener() {
		//
		// @Override
		// public void onClick(View arg0) {
		// // Intent intent=new
		// // Intent("jld.com.jld.remotecontrol.client.MainService");
		// // startService(intent);
		// // timer.schedule(task, 2000, 2000);
		// }
		// });

	}

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

				.setTitle("服务器地址")
				.setView(textEntryView)
				.setPositiveButton("好", new DialogInterface.OnClickListener() {
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
				.setNegativeButton("取消", new DialogInterface.OnClickListener() {
					public void onClick(DialogInterface dialog, int whichButton) {

					}
				}).create();
		dlg.show();
	}

	Handler handler = new Handler() {
		@Override
		public void handleMessage(Message msg) {
			// TODO Auto-generated method stub
			// 要做的事情

			SimpleDateFormat formatter = new SimpleDateFormat("HH:mm:ss");
			Date curDate = new Date(System.currentTimeMillis());// 获取当前时间
			String str = formatter.format(curDate);

			// textView1.setText(str);
			super.handleMessage(msg);
		}
	};

	/**
	 * A placeholder fragment containing a simple view.
	 */

}
