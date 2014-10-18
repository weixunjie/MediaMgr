package com.example.iot;

import android.util.Log;

import com.example.iot.port.State;

public class air_condition {
	private String id=null;
	private iot_interface  hardware;
	private state_observable  callback;
	 public enum State {
	       unknown, registered,unregisterd, opening,opened,closing,closed;
	    }
	private State state;
	
	public air_condition(String _id,iot_interface  _hardware,state_observable  _callback)
	{
		super();
		id=_id;
		hardware=_hardware;
		callback=_callback;
	}

	public int open()
	{
		byte[] val={'*',' ','r','f','B',' ','1','2','3','4',' ','6','2','2','0','1','2','3','4',' ','O','x',' ','A','x',' ','#'};
		byte[] bytes = id.getBytes();
		
		Log.e("air_condition", "open ");
		for(int i=0;i<bytes.length;i++)
		{
			val[11+i]=bytes[i];			
		}
		state=State.opening;
		return hardware.uart_send(val);
	}
	public int close()
	{	
		byte[] val={'*',' ','r','f','B',' ','1','2','3','4',' ','6','2','2','0','1','2','3','4',' ','C','x',' ','A','x',' ','#'};
		byte[] bytes = id.getBytes();
		Log.e("air_condition", "close ");
		for(int i=0;i<bytes.length;i++)
		{
			val[11+i]=bytes[i];			
		}
		state=State.closing;
		return hardware.uart_send(val);
	}
	
	public int set_tem_hot(int tmp)
	{	
		byte[] val={'*',' ','r','f','B',' ','1','2','3','4',' ','6','2','2','0','1','2','3','4',' ','A','H',' ','x','x',' ','#'};
		byte[] bytes = id.getBytes();
		Log.e("air-condition", "set_tem_cold  id="+id+" temp="+tmp);
		for(int i=0;i<bytes.length;i++)
		{
			val[11+i]=bytes[i];			
		}
		byte[] sb=String.format("%2d", tmp).getBytes();
		val[23]=sb[0];
		val[24]=sb[1];

		
	
		
		hardware.uart_send(val);
		
		try {
			Thread.sleep(1000);
		} catch (InterruptedException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		callback.state_callback(id, 0, 1);
		
		return 0;
	}
	
	public int set_tem_cold(int tmp)
	{	
		byte[] val={'*',' ','r','f','B',' ','1','2','3','4',' ','6','2','2','0','1','2','3','4',' ','A','C',' ','x','x',' ','#'};
		byte[] bytes = id.getBytes();
		Log.e("air-condition", "set_tem_cold  id="+id+"  temp="+tmp);
		for(int i=0;i<bytes.length;i++)
		{
			val[11+i]=bytes[i];			
		}
		byte[] sb=String.format("%2d", tmp).getBytes();
		val[23]=sb[0];
		val[24]=sb[1];
	
		hardware.uart_send(val);
		try {
			Thread.sleep(1000);
		} catch (InterruptedException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		callback.state_callback(id, 0, 1);
		return 0;
	}
	
	public int set_mode(int mode)
	{	
		byte[] val={'*',' ','r','f','B',' ','1','2','3','4',' ','6','2','2','0','1','2','3','4',' ','A','C',' ','x','x',' ','#'};
		byte[] bytes = id.getBytes();
		Log.e("air-condition", "set_mode  mode="+mode);
		for(int i=0;i<bytes.length;i++)
		{
			val[11+i]=bytes[i];			
		}

		if(mode==1)
		{
			val[21]='D';
		}
		else if(mode==2)
		{
			val[21]='F';
		}	
		else if(mode==3)
		{
			val[21]='A';
		}
		
		hardware.uart_send(val);
		try {
			Thread.sleep(1000);
		} catch (InterruptedException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		callback.state_callback(id, 0, 1);
		return 0;
	}
	
	public int get_state()
	{	
		byte[] val={'*',' ','r','f','C',' ','1','2','3','4',' ','6','2','2','0','1','2','3','4',' ','x','x',' ','x','x',' ','#'};
		byte[] bytes = id.getBytes();
		for(int i=0;i<bytes.length;i++)
		{
			val[11+i]=bytes[i];			
		}
		return hardware.uart_send(val);
		

	}
	
	public int received_message(byte[] val)
	{
		
		if( val[20]=='O')
		{
			state=State.opened;
			
			callback.state_callback(id, 0, 1);
			Log.e("iot_port", "the port uped in received_message");
		}
		else if(val[20]=='C') 
		{
			Log.e("iot_port", "the port offed in received_message");
			callback.state_callback(id, 0, 0);
		}
		else {
			//Log.e("iot_port", "msg error in received_message");
			
		}
		
		return 0;
	}

}
