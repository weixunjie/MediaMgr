package com.example.iot;

import android.R.integer;

public interface state_observable {
	public int  state_callback(String  device_id,int port,int state);
}
