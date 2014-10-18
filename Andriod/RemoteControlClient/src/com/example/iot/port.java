package com.example.iot;

import android.util.Log;


public class port {
	 public enum State {
	       unknown, registered,unregisterd, uping,uped,offing,offed;
	    }
	private State state;
	private  String id;
	private iot_interface  hardware;
	public port(String _id,iot_interface  _hardware)
	{
		super();
		hardware=_hardware;
		state=State.unknown;
		id=_id;	
	}
	public int power_up()
	{
		byte[] val={'*',' ','r','f','B',' ','1','2','3','4',' ','6','2','2','0','1','2','3','4',' ','O','x',' ','x','x',' ','#'};
		byte[] bytes = id.getBytes();
		
		Log.e("port", "power_up  id="+id);
		for(int i=0;i<bytes.length;i++)
		{
			val[11+i]=bytes[i];			
		}
		state=State.uping;
		return hardware.uart_send(val);
	}
	public int power_off()
	{	
		byte[] val={'*',' ','r','f','B',' ','1','2','3','4',' ','6','2','2','0','1','2','3','4',' ','C','x',' ','x','x',' ','#'};
		byte[] bytes = id.getBytes();
		Log.e("port", "power_off  id="+id);
		for(int i=0;i<bytes.length;i++)
		{
			val[11+i]=bytes[i];			
		}
		state=State.offing;
		return hardware.uart_send(val);
	}
	
	public int  get_state()
	{
		switch (state) {
		case unknown:
			return 0;
			
		case uping:
			return 1;
			
		case uped:
			return 2;
			
		case offing:
			return 3;
			
		case offed:
			return  4;
		default:
			break;
		}
	
		return -1;
	}
	
    public int  query_state()
    {
    	Log.e("pengyong","query_sate-call");
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
		
		if(val.length==26 && val[20]=='O')
		{
			state=State.uped;
			Log.e("iot_port", "the port uped in received_message");
			
		}
		else if(val.length==26 && val[20]=='C') 
		{
			Log.e("iot_port", "the port offed in received_message");
			state=State.offed;
		}
		else {
			Log.e("iot_port", "msg error in received_message");
			
		}
		
		return 0;
	}
}
