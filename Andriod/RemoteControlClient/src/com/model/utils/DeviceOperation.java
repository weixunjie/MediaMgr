package com.model.utils;

import java.util.ArrayList;

import android.R;
import android.content.Context;
import android.util.Log;

import com.datamodels.DeviceInfo;
import com.datamodels.DeviceStatus;
import com.example.iot.iot;

public class DeviceOperation {

	public static final String DEVICE_AC = "1";
	public static final String DEVICE_TV = "2";

	public static final String DEVICE_PROJECTOR = "3";

	public static final String DEVICE_PC = "4";

	public static final String DEVICE_LIGHT = "5";

	public static iot ioClass = null;

	public static ArrayList<String> regDevicdId = new ArrayList<String>();

	public static Context mContext = null;;

	public static DeviceStatus getStatus(String deviceId) {
		DeviceStatus ds = new DeviceStatus();
		return ds;
	}

	public static boolean setStatus(DeviceInfo diInfo, boolean isOpen,
			String acMode, String acTemp, boolean isSetMode) {

		if (!DeviceOperation.regDevicdId.contains(diInfo.getDeviceId())) {

			DeviceOperation.regDevicdId.add(diInfo.getDeviceId());
			ioClass.iot_register_device(diInfo.getDeviceId());

		}

		// if (regDevicdId.)
		// iot_hw.iot_register_device("1340409D");
		if (diInfo.isAC()) {

			if (isSetMode) {

				if (acMode.equals("1") || acMode.equals("2")) {
					ioClass.air_set_temp(diInfo.getDeviceId(), Integer.valueOf(acMode),
							Integer.valueOf(acTemp));
				}

			

				if (acMode.equals("3")) {

					ioClass.air_set_mode(diInfo.getDeviceId(), 3);
				}

				if (acMode.equals("4")) {

					ioClass.air_set_mode(diInfo.getDeviceId(), 1);
				}

				if (acMode.equals("5")) {

					ioClass.air_set_mode(diInfo.getDeviceId(), 2);
				}

			} else

			{

				ioClass.air_open_close(diInfo.getDeviceId(), isOpen);

			}

		} else {

			Log.e("wei", "device  id=" + diInfo.getDeviceId()
					+ "port wei number=" + diInfo.getPortNumber());

			ioClass.port_up_off(diInfo.getDeviceId(),
					Integer.valueOf(diInfo.getPortNumber()), isOpen);

		}

		

		return true;
	}
}
