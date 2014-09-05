package com.model.commandreceiver;

import android.util.Log;

public class CommandBean {
	
	public final static String TAG = "CommandReceiver";
	
	public String mUdpBroadcastAddress;
	
	public String mStreamName;
	
	public String mMediaType;
	
	public String mBitrate;
	
	public CommandBean(String add, String mediaType, String streamName, String bitrate) {
		this.mUdpBroadcastAddress = add;
		this.mStreamName = streamName;
		this.mMediaType = mediaType;
		this.mBitrate =  bitrate;
		
		Log.d(TAG, "address:" + add + ",streamName:" + mStreamName + ",type:" + mediaType + ", bitrate:" + bitrate);
	}

}
