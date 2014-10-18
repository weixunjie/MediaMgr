package com.example.iot;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;

import android.R.bool;
import android.R.integer;
import android.util.Log;

public class iot_interface {
	
    SerialPort sp;
	 FileOutputStream mOutputStream;
	 FileInputStream mInputStream;
	 iot_listener  listener;
	 iot_thread  mthread;
	boolean thread_stop=false;
	public iot_interface() {
	
		super();
		
	}
	public void  init( )
	{
		
		try {
			sp=new SerialPort(new File(new String("/dev/ttyS6")),9600);
			
			
		} catch (SecurityException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		
		
		 mInputStream=(FileInputStream) sp.getInputStream();
		 
		 mOutputStream=(FileOutputStream) sp.getOutputStream();

		 
	}
	public void deinit()
	{
		
		sp.uart_close();
	}
	
	void register_listener(iot_listener  _listenr)
	{
		listener=_listenr;	
	}
	
	private class iot_thread  extends Thread
	{
		
		 public void run() {
		         while(thread_stop)
		         {
		        	 byte[] val =new byte[255];
		        	 int len=uart_get(val);

		             Log.e("pengyong", "iot_thread value="+new String(val,0,len-1));
		        	 
		        	 listener.received_message(val);
		        	 
		         }
		    }
	}
	
	
	void iot_start()
	{
		thread_stop=true;
		mthread =new iot_thread();
		mthread.start();
		
	}
	void iot_stop()
	{
		thread_stop=false;
	}
	
	public int  uart_get(byte[] value)
	{
	
		int size=0;
		
		Log.e("pengyong", "uart_get in");
		try {
			size = mInputStream.read(value);
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		Log.e("pengyong", "uart_get out");
		return size;
	}	
	
	
	public int uart_send(byte[] value)
	{
		
		Log.e("pengyong", "uart_send in");
		try {
			mOutputStream.write(value);
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		Log.e("pengyong", "uart_send out");
		value.toString();
		return 0;
	}	
	
}
