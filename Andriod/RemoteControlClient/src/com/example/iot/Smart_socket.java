package com.example.iot;

import com.example.iot.port.State;

import android.R.integer;
import android.graphics.PorterDuff;
import android.util.Log;

public class Smart_socket {
	
	 public enum State {
	       unknown, registered, unregisterd;
	    }
	private State state;
	private port[] ports;
	private String id;
	private byte[] bytes ;
	private iot_interface  hardware;
	int port_number;
	private state_observable  callback ;
	public int get_port_number()
	{
		
		return bytes[2]-'0';
		
		
	}
	
	public Smart_socket(String _id,iot_interface  _hardware,state_observable _state_callback)
	{
		super();
		id=_id;
		hardware=_hardware;
		bytes= id.getBytes();
		port_number=get_port_number();
		ports=new port[port_number];
		callback=_state_callback;
		for(int i=0;i<port_number;i++)
		{
			bytes[2]=(byte)(i+'1');
			String port_id = new String(bytes);
			ports[i]=new port(port_id, _hardware);
		}
		callback=_state_callback;
		state=State.registered;
	}
	
	public State get_state()
	{
		return state;
		
	}
	public int  get_port_state(int number)
	{
		return ports[number].query_state();

	}
	
	public int query_port_state(int number)
	{
	
		return ports[number].query_state();
	}
	
	
	public int power_up_port(int number)
	{
		return ports[number].power_up();
		
	}
	
	public int power_off_port(int number)
	{
		
		return ports[number].power_off();
	}
	
	public int received_message(byte[] val)
	{
		if(val[2]=='r'&& val[3]=='f'&& val[4]=='F')
		 {	
		  ports[val[13]-'1'].received_message(val);
		  
		  Log.e("smart_socket", "length="+val.length+" val[20]="+val[20]);
		  
		  if( val[20]=='O')
		  {
			
			  callback.state_callback(id, val[13]-'1', 1);
		  }
		  else if(val[20]=='C') 
		  {
			  callback.state_callback(id, val[13]-'1', 0);
		  }
		 }
		return 0;
	}
	
	
}
