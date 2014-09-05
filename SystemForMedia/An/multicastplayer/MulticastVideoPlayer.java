package com.model.multicastplayer;

import java.io.File;

import com.model.commandreceiver.CommandReceiver;
import com.model.utils.Sharereference;

import android.app.Activity;
import android.app.AlertDialog;
import android.app.Dialog;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.SharedPreferences;
import android.media.AudioManager;
import android.media.MediaPlayer;
import android.os.Bundle;
import android.os.Environment;
import android.os.Handler;
import android.os.HandlerThread;
import android.os.Message;
import android.util.Log;
import android.view.ContextMenu;
import android.view.ContextMenu.ContextMenuInfo;
import android.view.Display;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.SurfaceHolder;
import android.view.SurfaceView;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.View.OnLongClickListener;
import android.view.WindowManager;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.TextView;
import android.widget.MediaController.MediaPlayerControl;
import android.widget.Toast;

public class MulticastVideoPlayer extends Activity implements MediaPlayer.OnPreparedListener, MediaPlayer.OnErrorListener,MediaPlayer.OnCompletionListener,MediaPlayer.OnInfoListener {
	/** Called when the activity is first created. */

    private MediaPlayer mediaPlayer;
    private String mMediaPath = "udp://229.0.0.1:1234"; //default value
    private String mMediaType = "video";                //default value  "audio" and "video"
    private int mMediaBitRates = 128*1024;               //bps
    
    public String mServerAddr;
    
    private SurfaceView surfaceView;
    private final static String TAG = "VodeoPlayActivity";
    private int prosition = 0;
    
    private int mDisplayWidth;
    private int mDisplayHeight;
    
	private static final int MESSAGE_BASE = 0;
    private static final int MESSAGE_PLAY = MESSAGE_BASE + 1;
    private static final int MESSAGE_STOP = MESSAGE_PLAY + 1;
    private static final int MESSAGE_RESTART = MESSAGE_STOP + 1;
    
    public static final String COMMAND_PLAYER_ERROR = "command_player_error";
    public static final String COMMAND_NETWORK_ERROR = "command_net_error";
    public static final String COMMAND_OTHER_ERROR = "command_other_error";
    public static final String COMMAND_CLIENT_QUIT = "command_quited";
    public static final String COMMAND_CLIENT_UPDATEADDR = "command_add_update";
    
    public static final String COMMAND_OPEN_TEST_TAG = "command_open_test_tag";
    
    public static final String COMMAND_PLAY_RESULT = "command_play_result";
    
    private boolean sendStartCommand = false;
    
    private boolean isPlaying = false;
    
    private int mPlayerstatus;
    
    private static final int STATUS_IDLE = 0;
    
    private static final int STATUS_PREPARED = 1;
    
    private static final int STATUS_PLAYING = 2;
    
    TextView mStartTime;
    TextView mCurrentTime;
    
    Context mContext = null;
    
    boolean mNeedPlay;
    
    View mControlDashBoard = null;
    boolean testLocalFile = false;
    boolean isTestVersion = false;
    
    private static int mClickTimes = -1;
    
    Handler myHandler = new Handler() {

		@Override
		public void handleMessage(Message msg) {
			// TODO Auto-generated method stub
			super.handleMessage(msg);
			
			switch (msg.what) {
			    case MESSAGE_PLAY:
			    	if (mPlayerstatus == STATUS_PLAYING || mPlayerstatus == STATUS_PREPARED) {
			    		stop();
			    	}
			    	play();
				    
			    break;
			
			    case MESSAGE_STOP:
			    	if (mPlayerstatus == STATUS_PLAYING || mPlayerstatus == STATUS_PREPARED) {
			    		stop();
			    	}
			    	finish();
			    break;
			    
			    case 1000:					
					long duration = mediaPlayer.getDuration();
					break;
			    case 1001:
			    	//mStartTime.setText("鎾斁鑰楁椂锛� + msg.arg1 + "ms");
			    	showCurrentPlayTime();
			        break;
			        
			    case 1002:
			    	//mCurrentTime.setText("褰撳墠鎾斁鏃堕棿锛� + msg.arg1 + "ms");
			    	showCurrentPlayTime();
			        break;
			    
			    default:
			    	
			    break;
			    
			}
		}
    	
    };
    
    
    private void showCurrentPlayTime() {
    	if (mediaPlayer != null && mediaPlayer.isPlaying()) {
    		int currentTime =  mediaPlayer.getCurrentPosition();
			Message msg = new Message();
			msg.what = 1002;
			msg.arg1 = currentTime;
			myHandler.sendMessageDelayed(msg,1000);
    	}
    }
    
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.videoplayer);

        surfaceView=(SurfaceView)this.findViewById(R.id.surfaceview);
        surfaceView.getHolder().setFixedSize(mDisplayHeight/*176*/, mDisplayWidth/*144*/);
        surfaceView.getHolder().setType(SurfaceHolder.SURFACE_TYPE_PUSH_BUFFERS);
        surfaceView.getHolder().addCallback(new SurceCallBack());
        surfaceView.setOnLongClickListener(new OnLongClickListener() {
			
			@Override
			public boolean onLongClick(View v) {
				// TODO Auto-generated method stub
				if (mControlDashBoard != null) {
					if (mControlDashBoard.isShown()) {
						mControlDashBoard.setVisibility(View.GONE);
					} else {
					    mControlDashBoard.setVisibility(View.VISIBLE);
					}
				}  
				return false;
			}
		});
        
         surfaceView.setOnClickListener(new OnClickListener() {
			
			@Override
			public void onClick(View v) {
				// TODO Auto-generated method stub
				mClickTimes ++;
				
				if (mClickTimes > 2) {
					showSettingDialog();
				}
				
				if (mClickTimes > 5) {
					notifyServer(COMMAND_OPEN_TEST_TAG);
					mClickTimes = 0;
				}
				
			}
		});
		
        IntentFilter filter =  new IntentFilter();
        filter.addAction(CommandReceiver.COMMAND_PLAY);
        filter.addAction(CommandReceiver.COMMAND_STOP);
        registerReceiver(mCommandListener, filter);
        
        //test 
        ButtonOnClikListiner buttonOnClikListinero = new ButtonOnClikListiner();
        ImageButton start = (ImageButton) this.findViewById(R.id.play);
//        ImageButton replay = (ImageButton) this.findViewById(R.id.reset);
        ImageButton server = (ImageButton) this.findViewById(R.id.server);
        ImageButton startserver = (ImageButton) this.findViewById(R.id.startService);
        
        mStartTime = (TextView) findViewById(R.id.startTime);
        mCurrentTime = (TextView) findViewById(R.id.currentTime);
        
        start.setOnClickListener(buttonOnClikListinero);
//        replay.setOnClickListener(buttonOnClikListinero);
        server.setOnClickListener(buttonOnClikListinero);
        startserver.setOnClickListener(buttonOnClikListinero);
        
        mControlDashBoard = findViewById(R.id.contradashbord);
        mContext = this;
        
        mPlayerstatus = STATUS_IDLE;  
        
        isTestVersion = isTestVersion();
    }
    
    @Override
	protected void onResume() {
		// TODO Auto-generated method stub
		super.onResume();
        
        if (!Sharereference.getIsOneMoreTime(mContext,false)) {
        	showSettingDialog();
        }
	}

	private void getDisplay() {
    	WindowManager manage = getWindowManager();
        Display display=manage.getDefaultDisplay();
        mDisplayWidth = display.getWidth() > 0 ? display.getWidth() : 176;
        mDisplayHeight = display.getHeight() > 0? display.getWidth() : 144;
    }
    
    private final class ButtonOnClikListiner implements View.OnClickListener {
        @Override
        public void onClick(View v) {

            switch (v.getId()) {
            case R.id.play:
            	if (isPlaying) {
            		stop();
            		((ImageButton)v).setImageResource(R.drawable.icn_media_play_pressed_holo_dark);
            	} else {
                    play();
                    ((ImageButton)v).setImageResource(R.drawable.icn_media_pause_pressed_holo_dark);
            	}
                break;
            case R.id.startService:
                Intent intent = new Intent();
                intent.setClass(mContext, CommandReceiver.class);
                startService(intent);               
                break;
/*            case R.id.reset:
            	
            	if (mediaPlayer != null) {
            		mediaPlayer.reset();
            		mediaPlayer.release();
            		mediaPlayer = null;
            	}
//                if(mediaPlayer.isPlaying()){
//                    mediaPlayer.seekTo(0);
//                }else{
//                    play();
//                }
                break;*/
            case R.id.server:
				if (!sendStartCommand) {
					sendStartCommand = true;
					String command = "multicastplayer_play";
					Intent i = new Intent();
					i.setAction(command);
					sendBroadcast(i);
					((ImageButton)v).setImageResource(R.drawable.icn_media_pause_pressed_holo_dark);
            	} else {
            		sendStartCommand = false;
            		String command = "multicastplayer_stop";
					Intent i = new Intent();
					i.setAction(command);
					sendBroadcast(i);
					((ImageButton)v).setImageResource(R.drawable.icn_media_play_pressed_holo_dark);
            	}
                break;
            	
            }
        }  
    }
    
    public void sendResultToServer(String command,String errCode) {
		Intent i = new Intent();
		i.setAction(command);
		i.putExtra("errcode", errCode);
		sendBroadcast(i);
    }
    
    private void startCommandServer(Context context) {
    	  Intent intent = new Intent();
          intent.setClass(context, CommandReceiver.class);
          startService(intent);
    }
    
    
    private void notifyServer(String command) {
		Intent i = new Intent();
		i.setAction(command);
		sendBroadcast(i);
    }
    
    private boolean isTestVersion() {
    	boolean ret = false;
    	
    	File mFile = new File("sdcard/mt.txt");
    	
    	if (mFile.exists()) {
    		ret = true;
    	}
    	return ret;
    }
    
    @Override
	protected void onNewIntent(Intent intent) {
		// TODO Auto-generated method stub
		super.onNewIntent(intent);
		Log.i(TAG, "new intent");
		initParams(intent);
	}
    
    
    private void initParams(Intent intent) {
		
		if (intent != null ) {			
			mMediaPath = intent.getStringExtra("address");			
			mMediaType = intent.getStringExtra("mediaType");
			mMediaBitRates = intent.getIntExtra("bitrate", 2000000);
			
			if (mMediaPath ==  null || "".equals(mMediaPath)) {
				mNeedPlay = false;
				return;
			}
			mNeedPlay = true;
			Log.d(TAG, "mMediaPath:" + mMediaPath + ",mMediaType:" + mMediaType + ",mMediaBitRates:" + mMediaBitRates);
		}
    }
    
    private BroadcastReceiver mCommandListener = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
        	
            String action = intent.getAction();
            Log.i(TAG,"onReceive action :" + action);
            
            if (action.equals(CommandReceiver.COMMAND_PLAY)) {   
                initParams(intent);
                myHandler.sendEmptyMessage(MESSAGE_PLAY);
            } else if (action.equals(CommandReceiver.COMMAND_STOP)) {
            	myHandler.sendEmptyMessage(MESSAGE_STOP);
            } 
        }
    };



	private void play() {
		try {	
		       
			mediaPlayer = new MediaPlayer();
			mediaPlayer.setOnPreparedListener(this);
			mediaPlayer.setOnErrorListener(this);
			mediaPlayer.setOnInfoListener(this);
			mediaPlayer.setOnCompletionListener(this);
			//mediaPlayer.reset();
			mediaPlayer.setAudioStreamType(AudioManager.STREAM_MUSIC);
			mediaPlayer.setDisplay(surfaceView.getHolder());
			
			if (mMediaPath == null) {
//				mMediaPath = "udp://238.0.0.1:1234-video/2000000";
				mMediaPath = "udp://229.0.0.1:1236-audio/320";
				if (testLocalFile) {				
					mMediaPath = "/sdcard/afanda_clip.mov";
				}
			}
			mediaPlayer.setDataSource(mMediaPath);
			mediaPlayer.prepareAsync();
			
			mPlayerstatus = STATUS_PREPARED;
			isPlaying = true;
			Log.d(TAG, "setDataSource success!");
			// mediaPlayer.start();
		} catch (Exception e) {
			Log.e(TAG, e.toString());
			e.printStackTrace();
		}
	}
	
	private void stop() {
		try {
			isPlaying = false;
			mPlayerstatus = STATUS_IDLE;
			if (mediaPlayer != null) {
				mediaPlayer.reset();
				mediaPlayer.release();
				mediaPlayer = null;
			}
			
		} catch (Exception ex) {
			
		}
		sendResultToServer(COMMAND_PLAY_RESULT,"0");
	}
    
    private final class SurceCallBack implements SurfaceHolder.Callback{
        /**
         * 閿熸枻鎷烽敓鏂ゆ嫹閿熺潾闈╂嫹
         */
        @Override
        public void surfaceChanged(SurfaceHolder holder, int format, int width,
                int height) {
            // TODO Auto-generated method stub
            mDisplayHeight = height;
            mDisplayWidth = width;
        }

        /**
         * 閿熸枻鎷烽敓鑺ュ垱閿熸枻鎷�
         */
        @Override
        public void surfaceCreated(SurfaceHolder holder) {
            
            if (getIntent() != null) {
        		initParams(getIntent());
        		if (mNeedPlay || isTestVersion) {
        		    myHandler.sendEmptyMessage(MESSAGE_PLAY);
        		}
            }
        }

        /**
         * 閿熸枻鎷烽敓鏂ゆ嫹閿熸枻鎷烽敓锟�
         */
        @Override
        public void surfaceDestroyed(SurfaceHolder holder) {
        	
        }
    }

    @Override
	public boolean onCreateOptionsMenu(Menu menu) {
		// TODO Auto-generated method stub
		MenuInflater inflater = getMenuInflater();
		inflater.inflate(R.menu.menus, menu);
		return super.onCreateOptionsMenu(menu);
	}
    
    @Override  
    public boolean onOptionsItemSelected(MenuItem item) {  
    	showSettingDialog();
        return super.onOptionsItemSelected(item);  
    }  
    
    @Override  
    public void onCreateContextMenu(ContextMenu menu, View v,  
            ContextMenuInfo menuInfo) {  
        MenuInflater inflater = getMenuInflater();  
        inflater.inflate(R.menu.menus, menu);  
        super.onCreateContextMenu(menu, v, menuInfo);  
    }  
    
    @Override  
    public boolean onContextItemSelected(MenuItem item){  
        Toast.makeText(this, String.valueOf(item.getItemId()), Toast.LENGTH_LONG).show();  
        return super.onContextItemSelected(item);  
    }  

	@Override
	public void onPrepared(MediaPlayer mp) {
		// TODO Auto-generated method stub
		Log.i(TAG, "reonPrepared");
		mPlayerstatus = STATUS_PLAYING;
		mediaPlayer.start();
		myHandler.sendEmptyMessageDelayed(1000, 1000);
		sendResultToServer(COMMAND_PLAY_RESULT,"0");
	}

	@Override
	protected void onPause() {
		// TODO Auto-generated method stub
		super.onPause();
	}

	@Override
	protected void onDestroy() {
		// TODO Auto-generated method stub
		super.onDestroy();
		
		unregisterReceiver(mCommandListener);
		
		if (mediaPlayer != null) {
			mediaPlayer.release();
			mediaPlayer = null;
		}
	}
	 
	@Override
	public boolean onError(MediaPlayer mp, int what, int extra) {
		// TODO Auto-generated method stub
		
		Log.i(TAG, "receive err info : what = " + what + ",extra = " + extra);
		if (mediaPlayer != null) {
		    mediaPlayer.reset();
		}
		
		
		if (what == 100) {
			Toast.makeText(mContext, R.string.playererr, Toast.LENGTH_SHORT).show();		
			notifyServer(COMMAND_PLAYER_ERROR);
		} else if (what == 1 && extra < 0 && extra > -6) {
			Toast.makeText(mContext, R.string.networkerr, Toast.LENGTH_SHORT).show();
			notifyServer(COMMAND_NETWORK_ERROR);
			myHandler.sendEmptyMessageDelayed(MESSAGE_PLAY, 5000000);
			return false;
		} else {
			Toast.makeText(mContext, R.string.playererr, Toast.LENGTH_SHORT).show();		
			notifyServer(COMMAND_OTHER_ERROR);
		}
		sendResultToServer(COMMAND_PLAY_RESULT,"playfailed");
		finish();
		return false;
	}

	@Override
	public void onCompletion(MediaPlayer mp) {
		// TODO Auto-generated method stub
		Log.e(TAG, "onCompletion  stream get end");
		if (mediaPlayer != null) {
		    stop();
		}
		if (testLocalFile) {				
			play();
		}
}
	
	private void showSettingDialog() {
		LayoutInflater factory = LayoutInflater.from(mContext);
        final View textEntryView = factory.inflate(R.layout.serversetting, null);
        
        EditText addrEdit = (EditText) textEntryView.findViewById(R.id.addr_edit);
        addrEdit.setText(Sharereference.getMulticastAddr(mContext, ""));
        
        AlertDialog dlg = new AlertDialog.Builder(mContext)
       
        .setTitle(mContext.getString(R.string.serversetting))
        .setView(textEntryView)
        .setPositiveButton(mContext.getString(R.string.ok), new DialogInterface.OnClickListener() {
            public void onClick(DialogInterface dialog, int whichButton) {
              EditText addrEdit = (EditText) textEntryView.findViewById(R.id.addr_edit);
              String addr = addrEdit.getText().toString(); 
//              EditText ports = (EditText) textEntryView.findViewById(R.id.ports_edit);
              if (addr != null && !"".equals(addr)/* && ports != null && !"".equals(ports)*/) {
            	  mServerAddr = addr/* + ports*/;
                  Sharereference.setPathAddr(mContext,addr);
                  Sharereference.setIsOneMoreTime(mContext,true);
                  if (Sharereference.isServiceRunning(mContext)) {
                	  notifyServer(COMMAND_CLIENT_UPDATEADDR);
                	  
                  } else {
                	  startCommandServer(mContext);
                  }
                  Toast.makeText(mContext, mContext.getString(R.string.serversettingok), Toast.LENGTH_LONG).show();  
                  finish();
              } else {
                  Dialog mDialog = new MyDialog(mContext,
                          R.style.MyDialog);
                  mDialog.show();
              }
            }
        }).setNegativeButton(mContext.getString(R.string.cancel), new DialogInterface.OnClickListener() {
            public void onClick(DialogInterface dialog, int whichButton) {
            	
            }
        }).create();
        dlg.show();
	}
	
	public class MyDialog extends Dialog {

	    Context context;
	    MyDialog me;
	    public MyDialog(Context context) {
	        super(context);
	        // TODO Auto-generated constructor stub
	        this.context = context;
	        me = this;
	    }
	    public MyDialog(Context context, int theme){
	        super(context, theme);
	        this.context = context;
	    }
	    
	    @Override
	    protected void onCreate(Bundle savedInstanceState) {
	        // TODO Auto-generated method stub
	        super.onCreate(savedInstanceState);
	        this.setContentView(R.layout.dialogcontent);
	        Button bt =  (Button) this.findViewById(R.id.dialog_button_cancel);
	        bt.setOnClickListener(new android.view.View.OnClickListener() {
				
				@Override
				public void onClick(View arg0) {
					// TODO Auto-generated method stub	
					dismiss();
				}
			});
	    }
	}
		@Override
	public boolean onInfo(MediaPlayer mp, int what, int extra) {
		// TODO Auto-generated method stub
		Log.i(TAG, "onInfo arg1:" + what + "arg2:" + extra);
		if (what == 1000) {
			Message msg = new Message();
			msg.what = 1001;
			msg.arg1 = extra/1000;
			myHandler.sendMessage(msg);
		} else if (what == 1001 && extra == 0000) {
			myHandler.sendEmptyMessage(MESSAGE_PLAY);
			
		}
		return false;
	}	

	
}
