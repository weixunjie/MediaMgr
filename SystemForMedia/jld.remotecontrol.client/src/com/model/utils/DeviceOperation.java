package com.model.utils;

import com.datamodels.DeviceInfo;
import com.datamodels.DeviceStatus;
import com.example.iot.iot;



public class DeviceOperation {

	public static final String DEVICE_AC = "1";
	public static final String DEVICE_TV = "2";

	public static final String DEVICE_PROJECTOR = "3";

	public static final String DEVICE_PC = "4";

	public static final String DEVICE_LIGHT = "5";
	

	public static iot ioClass =null;;
	public static DeviceStatus getStatus(String deviceId) {
		DeviceStatus ds=new DeviceStatus();
		return ds;
	}	
	
	public static boolean setStatus(DeviceInfo diInfo,boolean isOpen,String acMode, String acTemp) {
			
		if (diInfo.isAC()) {
			ioClass.air_open_close(diInfo.getDeviceId(), isOpen);
			
		} else {
		
			ioClass.port_up_off(diInfo.getDeviceId(),
					Integer.valueOf(diInfo.getPortNumber()), isOpen);
		}
		
		if (!acMode.equals("") && !acTemp.equals(""))
		{
			ioClass.air_set_temp(diInfo.getDeviceId(), Integer.valueOf( acMode),Integer.valueOf(acTemp));
		}
		
		
		
		return true;
	}	
	
	
	public static String getDeviceDisplayByType(String deviceType) {
		if (deviceType.equals(DEVICE_AC))
		{
			return "空调";
		}
		else if (deviceType.equals(DEVICE_TV))
		{
			return "电视";
		}
		else if (deviceType.equals(DEVICE_PROJECTOR))
		{
			return "投影仪";
		}
		else if (deviceType.equals(DEVICE_PC))
		{
			return "电脑";
		}
		else if (deviceType.equals(DEVICE_LIGHT))
		{
			return "灯";
		}
		
		return "";
	}	
	

}
