package com.datamodels;

public class DeviceStatus {
	private String deviceId;

	private boolean isOpen;

	private String acTempure;

	private String acMode;

	public String getDeviceId() {
		return deviceId;
	}

	public void setDeviceId(String deviceId) {
		this.deviceId = deviceId;
	}

	public boolean isOpen() {
		return isOpen;
	}

	public void setOpen(boolean isOpen) {
		this.isOpen = isOpen;
	}

	public String getAcMode() {
		return acMode;
	}

	public void setAcMode(String acMode) {
		this.acMode = acMode;
	}

	public String getAcTempure() {
		return acTempure;
	}

	public void setAcTempure(String acTempure) {
		this.acTempure = acTempure;
	}

}
