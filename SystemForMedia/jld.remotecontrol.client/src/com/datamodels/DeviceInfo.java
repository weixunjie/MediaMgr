package com.datamodels;

import java.io.Serializable;

public class DeviceInfo implements Serializable {
	private String deviceId;
	private String deviceType;
	private String deviceDisplay;
	
	private boolean isAC;
	private String portNumber;
	
	private String acTempure;
	
	private String acMode;

	public String getDeviceId() {
		return deviceId;
	}

	public void setDeviceId(String deviceId) {
		this.deviceId = deviceId;
	}

	public String getDeviceType() {
		return deviceType;
	}

	public void setDeviceType(String deviceType) {
		this.deviceType = deviceType;
	}

	public String getDeviceDisplay() {
		return deviceDisplay;
	}

	public void setDeviceDisplay(String deviceDisplay) {
		this.deviceDisplay = deviceDisplay;
	}

	public String getPortNumber() {
		return portNumber;
	}

	public void setPortNumber(String portNumber) {
		this.portNumber = portNumber;
	}

	public String getAcTempure() {
		return acTempure;
	}

	public void setAcTempure(String acTempure) {
		this.acTempure = acTempure;
	}

	public String getAcMode() {
		return acMode;
	}

	public void setAcMode(String acMode) {
		this.acMode = acMode;
	}

	public boolean isAC() {
		return isAC;
	}

	public void setAC(boolean isAC) {
		this.isAC = isAC;
	}


}
