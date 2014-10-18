package com.datamodels;

import java.io.Serializable;

public class DeviceOperationQueue implements Serializable {
	private DeviceInfo deviceInfo;
	private boolean isOpen;

	private boolean isSetMode;
	
	public DeviceInfo getDeviceInfo() {
		return deviceInfo;
	}

	public void setDeviceInfo(DeviceInfo deviceInfo) {
		this.deviceInfo = deviceInfo;
	}

	public boolean isOpen() {
		return isOpen;
	}

	public void setOpen(boolean isOpen) {
		this.isOpen = isOpen;
	}

	public boolean isSetMode() {
		return isSetMode;
	}

	public void setSetMode(boolean isSetMode) {
		this.isSetMode = isSetMode;
	}

}
