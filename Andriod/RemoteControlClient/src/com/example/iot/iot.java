package com.example.iot;

import java.util.HashMap;
import java.util.Iterator;
//import java.util.Timer;
//import java.util.TimerTask;


import android.util.Log;

public class iot implements iot_listener {
	private iot_interface  hardware;
	private HashMap<String, Smart_socket> msocket;
	private HashMap<String, Smart_socket> msocket_back;
	private HashMap<String, air_condition> air_cond;
	private HashMap<String, air_condition> air_cond_back;
	String version=null;
//	 private Timer timer=null;
//	 private TimerTask task =null;
	 private static iot_observable  obser=null;
	 public static state_observable  state_callback=null ;
	 public enum State {
	       unknown,registering,registered,unregistering,unregisterd,set_freqing,set_freqed,get_freqing,get_freqed,get_softing,get_softed;
	    }
   // private  State state;
	 private  int state;
	//private Smart_socket[] sockets; 
	
	public iot(iot_observable  observable   ,state_observable  _state_callback )
	{
			super();
			hardware=new iot_interface();
			msocket=new HashMap<String, Smart_socket>();
			msocket_back=new HashMap<String, Smart_socket>();
			
			air_cond=new HashMap<String, air_condition> ();
			air_cond_back=new HashMap<String, air_condition> ();
			state |= 1<<(State.unknown.ordinal());
			obser=observable;
			state_callback=_state_callback;
			
			
//			timer = new Timer( );
//			task = new TimerTask( ) {z
//			 public void run ( ) {
//			   unregister();
//			 }};		
	}
	
	public void  iot_register_device(String id)
	{
		byte[] value=String.format("%s", id).getBytes();
		
		if(value[0]=='0'&&value[1]=='9')
		{
			 air_condition  air =new air_condition(id, hardware,state_callback);
			 air_cond.put(id, air);
			 air_cond_back.put(id, air);
			
		}
		else if((value[0]=='1'&&value[1]=='3')||(value[0]=='6'&&value[1]=='2'))
		{
			Smart_socket socket= new Smart_socket(id, hardware,state_callback);
			msocket.put(id, socket);
			msocket_back.put(id, socket);	
			Log.e("iot", "port number"+msocket.get(id).get_port_number());
		}
	    return ;
	}
	
	public void init()
	{
		hardware.init();
		hardware.register_listener(this);
		hardware.iot_start();
		
	}
	
	public int getState() {
		return state;
	}
	
	public void deinit()
	{
		hardware.iot_stop();
		hardware.deinit();
	}
	public int register()
	{
		byte[] val={'*',' ','r','f','J',' ','1','2','3','4',' ','x','x','x','x','x','x','x','x',' ','x','x',' ','x','x',' ','#'};
		//state=State.registering;
		Log.e("iot", "register");
		state |= 1<<(State.registering.ordinal());
		state &= ~(1<<(State.registered.ordinal()));
		  hardware.uart_send(val); 
		//timer.schedule(task,50000);
		 return 0;
 
	}
	
	public  int  unregister()
	{
		byte[] val={'*',' ','r','f','K',' ','1','2','3','4',' ','x','x','x','x','x','x','x','x',' ','x','x',' ','x','x',' ','#'};
		//state=State.unregistering;
		Log.e("iot", "unregister");
		state |= 1<<(State.unregistering.ordinal());
		state &= ~(1<<(State.unregisterd.ordinal()));
		return hardware.uart_send(val);
	}
	
	  private String get_value(byte[]value, int start,int stop)
      {

                      String str;
                      str=new String(value, start, stop-start+1);
                      //re=Integer.valueOf(str, 16);
                      return str;

              }

	public int  received_message(byte[] value)
	{
		
		Log.e("iot", "received_message="+get_value(value, 0, 25));
		
		if(get_value(value,0,9).equals("* rfm 1234"))
		{
			Log.e("pengyong", "iot state is begin to set_frequed!");
			//state=State.set_freqed;
			state |= 1<<(State.set_freqed.ordinal());
			return 0;
		}
		else if(get_value(value,0,9).equals("* rfh 1234"))
		{
		//	state=State.get_freqed;
			state |= 1<<(State.get_freqed.ordinal());
			Log.e("pengyong", "iot state is begin to get_frequed!");
			return 0;
		}
		else if(get_value(value,0,9).equals("* rfv 1234"))
		{
			int i=0;
			for(i=11;i<19;i++)
			{
				if(value[i]=='0')
				{
					continue;
			     }
				else {
					break;
				}
			
		     }
			if(i==19)
			{	
				state |= 1<<(State.get_softed.ordinal());
				Log.e("pengyong", "iot state is begin to get_softed!");
				version=get_value(value, 20, 25);
			}
			else
			{
				Log.e("pengyong", "get  iot_device soft version is returned");
			//s	String id= get_value(value, 11, 18);
				//msocket_back.get(id).received_message(value);
			}
			return 0;
		}
		else if(get_value(value,0,9).equals("* rfj 1234"))
		{
			Log.e("pengyong", "iot state is begin to registed!");
		//	state |= 1<<State.registered;
			state |= 1<<(State.registered.ordinal());
			return 0;
		}
		else if(get_value(value,0,9).equals("* rfk 1234"))
		{
			Log.e("pengyong", "iot state is begin to unregisted!");
			//state=State.unregisterd;
			state |= 1<<(State.unregisterd.ordinal());
			return 0;
		}
		
		else if(get_value(value,0,4).equals("* rff"))
		{
			String id =get_value(value,11,18);
			boolean  find=false;
			Iterator iterator=null;
			Log.e("iot", "received message value="+get_value(value, 0, 10));
			
			if(value[11]=='0'&&value[12]=='9')
			{
				 iterator = air_cond.keySet().iterator();
			}
			else if((value[11]=='1'&&value[12]=='3')||(value[11]=='6'&&value[12]=='2'))
			{
				 iterator = msocket.keySet().iterator();
				 Log.e("iot", "socket device is registed\n");
			}
			
			while(iterator.hasNext()) {
				String sid=(String)iterator.next();
				Log.e("iot ", "sid="+sid+" id="+id);
				
				if(sid.equals(id))
				{
					find=true;
					Log.e("iot","the device is registed  id="+id);
					break;
				}
				
			}
			if(find==false)
			{
				Log.e("iot","new device is reigisted id="+id);
				
			
				
				if(value[11]=='0'&&value[12]=='9')
				{
					 air_condition  air =new air_condition(id, hardware,state_callback);
					 air_cond.put(id, air);
					 air_cond_back.put(id, air);
					 obser.device_registered_callback(id,1);
				}
				else if((value[11]=='1'&&value[12]=='3')||(value[11]=='6'&&value[12]=='2'))
				{
					Smart_socket socket= new Smart_socket(id, hardware,state_callback);
					msocket.put(id, socket);
					msocket_back.put(id, socket);	
					obser.device_registered_callback(id,msocket.get(id).get_port_number());
					Log.e("iot", "port number"+msocket.get(id).get_port_number());
				}
				
				
			}
			
		}
		else 
			{
			//String id = get_value(value, 11, 18);
			Iterator iterator=null;
			if(value[11]=='0'&&value[12]=='9')
			{
				 iterator = air_cond_back.keySet().iterator();
			}
			else if((value[11]=='1'&&value[12]=='3')||(value[11]=='6'&&value[12]=='2'))
			{
				 iterator = msocket_back.keySet().iterator();
			}
			
			while(iterator.hasNext()) {
				String sid=(String)iterator.next();
				Log.e("iot", "  sid="+sid);
				 byte[] sb=String.format("%s", sid).getBytes();
				 
				 if(sb[0]==value[11]&&sb[1]==value[12]&&sb[3]==value[14]&&sb[4]==value[15]&& sb[5]==value[16]&&sb[6]==value[17]&&sb[7]==value[18])
				 {
					 Log.e("iot", "sid message received"+sid);
					 
					 if(value[11]=='0'&&value[12]=='9')
						{
						 	air_condition  air=air_cond_back.get(sid); 	
							air.received_message(value);
						}
					 else if((value[11]=='1'&&value[12]=='3')||(value[11]=='6'&&value[12]=='2'))
						{
							Smart_socket socket_hw  =msocket_back.get(sid);
							socket_hw.received_message(value);
						}
					 
				 }else {
					continue;
				}
			
			}
			
			
		}
				
		return 0;
	}
	
	public int get_state(String id,  int port_number )
	{
		
		byte[] sb=String.format("%s", id).getBytes();
		 if(sb[0]=='0'&&sb[1]=='9')
		 {
			 Iterator iterator = air_cond.keySet().iterator();
				while(iterator.hasNext())
				{
					String sid=(String)iterator.next();
					Log.e("iot", "  sid="+sid);

					 if(sid.equals(id))
					 {	
						 Log.e("iot", "find sid deviced "+sid);
						 air_condition air=null;
						 air=air_cond.get(sid);			
						 air.get_state();
						 
						 }
				 }	
			 	
		 }
		 else if(((sb[0]=='1'&&sb[1]=='3')||(sb[0]=='6'&&sb[1]=='2')))
		 {
				Iterator iterator = msocket.keySet().iterator();
				while(iterator.hasNext())
				{
					String sid=(String)iterator.next();
					Log.e("iot", "  sid="+sid);

					 if(sid.equals(id))
					 {	
						 
//						   byte[] val={'*',' ','r','f','C',' ','1','2','3','4',' ','x','x','x','x','x','x','x','x',' ','x','x',' ','x','x',' ','#'};
//							//state=State.registering;
//							Log.e("iot", "register");			
//							hardware.uart_send(val); 
//							return 0;
						 int times=0;
						 Log.e("iot", "find sid deviced "+sid);
						 Smart_socket socket_hw=null;
						 socket_hw=msocket.get(sid);
						 int total_number=socket_hw.get_port_number();
						 if(port_number<=total_number)
						 {
							 
							 socket_hw.get_port_state(port_number);
					     
						 }
						
						 
						 
					}
				}

			 
		 }
		 else {
			return -1;
		}
	
		 return 0;
	}
	
	
	public int set_frequency(int net_id,int freq)
	{
		
		 byte[] result1=new byte[2];
		 byte[] result2=new byte[2];
		 result1= String.format("%02x", net_id).getBytes();
		 result2= String.format("%02x", freq).getBytes();
		 byte[] val={'*',' ','r','f','M',' ','1','2','3','4',' ','x','x','x','x','x','x','x','x',' ',result1[0],result1[1],' ',result2[0],result2[1],' ','#'};
		// state=State.set_freqing;
		 state |= 1<<(State.set_freqing.ordinal());
		 state &= ~(1<<(State.set_freqed.ordinal()));
		 return hardware.uart_send(val);
	}
	
	public int query_frequency()
	{
		 byte[] val={'*',' ','r','f','H',' ','1','2','3','4',' ','x','x','x','x','x','x','x','x',' ','x','x',' ','x','x',' ','#'};
		// state=State.get_freqing;
		 state |= 1<<(State.get_freqing.ordinal());
		 state &= ~(1<<(State.get_freqed.ordinal()));
		 return hardware.uart_send(val);
	}
	
	public int query_software_version()
	{
		byte[] val={'*',' ','r','f','V',' ','1','2','3','4',' ','0','0','0','0','0','0','0','0',' ','x','x',' ','x','x',' ','#'};
		// state=State.get_softing;
		 state |= 1<<(State.get_softing.ordinal());
		 state &= ~(1<<(State.get_softed.ordinal()));
		 return hardware.uart_send(val);
		
	}
	
//	public String get_device_ids()
//	{
//		String result  =  null;
//		Iterator iterator = msocket.keySet().iterator();
//		while(iterator.hasNext()) {
//			String sid=(String)iterator.next();
//			Log.e("iot", "  sid="+sid);
//			if(result!=null){
//				 result=String.format("%s:%s",result ,sid);
//			}
//			else {
//				result=String.format("%s", sid) ;
//			}
//		}
//		
////		 iterator = air_cond.keySet().iterator();
////		while(iterator.hasNext()) {
////			String sid=(String)iterator.next();
////			Log.e("iot", "  sid="+sid);
////			if(result!=null){
////				 result=String.format("%s:%s",result ,sid);
////			}
////			else {
////				result=String.format("%s", sid) ;
////			}
////		}
//		
//		return result;
//	}
	public int air_open_close(String id, boolean control)
	{
		byte[] sb=String.format("%s", id).getBytes();
		 if(sb[0]!='0'&&sb[1]!='9')
		 {
			 	Log.e("iot", "port_control can,t supoort this function for "+id);
			 	return -1;
		 }
			Iterator iterator = air_cond.keySet().iterator();
			while(iterator.hasNext())
			{
				String sid=(String)iterator.next();
				Log.e("iot", "  sid="+sid);

				 if(sid.equals(id))
				 {	
					 Log.e("iot", "find sid deviced "+sid);
					 air_condition air=null;
					 air=air_cond.get(sid);
				
						 
					     if(control){
					    	 air.open();
					     }
					     else {
					    	air.close();
						}
				     
					 }
			 }
			
		
		return 0;
	}
	public int air_set_mode(String id, int mode)
	{
		byte[] sb=String.format("%s", id).getBytes();
		 if(!(sb[0]=='0'&&sb[1]=='9')|| mode<1 || mode >3)
		 {
			 	Log.e("iot", "port_control can,t supoort this function for "+id);
			 	return -1;
		 }
			Iterator iterator = air_cond.keySet().iterator();
			while(iterator.hasNext())
			{
				String sid=(String)iterator.next();
				Log.e("iot", "  sid="+sid);

				 if(sid.equals(id))
				 {	
					 Log.e("iot", "find sid deviced "+sid);
					 air_condition air=null;
					 air=air_cond.get(sid);				
					 air.set_mode(mode);			     
					 }
			 }
			
		
		return 0;
	}
	
	public int air_set_temp(String id, int mode,int temp)
	{
		byte[] sb=String.format("%s", id).getBytes();
		 if(!(sb[0]=='0'&&sb[1]=='9')|| mode<1 || mode >2)
		 {
			 	Log.e("iot", "port_control can,t supoort this function for "+id);
			 	return -1;
		 }
			Iterator iterator = air_cond.keySet().iterator();
			while(iterator.hasNext())
			{
				String sid=(String)iterator.next();
				Log.e("iot", "  sid="+sid);

				 if(sid.equals(id))
				 {	
					 Log.e("iot", "find sid deviced "+sid);
					 air_condition air=null;
					 air=air_cond.get(sid);	
					 if(mode==1&& temp%2==0 && temp>=20 && temp<=30)
					 {
						 air.set_tem_hot(temp);	   
					 }
					 else
					 if(mode==2&& temp%2==0 && temp>=16 && temp<=30)
					{
							 air.set_tem_cold(temp);	   
					}
					 else {
						Log.e("iot air set temp", "temp is error");
					}
				}
			 }
		return 0;
	}
	
		
	public int port_up_off(String id, int port_number,boolean control)
	{
		int ret=-1;
		//int port_should_value= control?  2: 4;
		Log.e("iot", "port_control  id="+id+"port_number="+port_number);
		
		 byte[] sb=String.format("%s", id).getBytes();
		 
		 if(!((sb[0]=='1'&&sb[1]=='3')||(sb[0]=='6'&&sb[1]=='2')))
		 {
			 	Log.e("iot", "port_control can,t supoort this function for "+id);
			 	return -1;
		 }
		 
		Iterator iterator = msocket.keySet().iterator();
		while(iterator.hasNext())
		{
			String sid=(String)iterator.next();
			Log.e("iot", "  sid="+sid);

			 if(sid.equals(id))
			 {		
				 int times=0;
				 Log.e("iot", "find sid deviced "+sid);
				 Smart_socket soceket_hw=null;
				 soceket_hw=msocket.get(sid);
				 int total_number=soceket_hw.get_port_number();
				 if(port_number<=total_number)
				 {
					 
				     if(control){
				    	 soceket_hw.power_up_port(port_number);
				     }
				     else {
				    	 soceket_hw.power_off_port(port_number);
					}
			     
				 }
				
				 
				 
			}
		}

		return ret;
	}
	
	
	
	
}
