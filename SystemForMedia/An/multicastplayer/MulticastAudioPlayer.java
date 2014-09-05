package com.model.multicastplayer;

import java.io.FileDescriptor;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;

import android.content.Context;
import android.content.Intent;
import android.media.AudioManager;
import android.media.MediaPlayer;
import android.os.Handler;
import android.util.Log;
import android.widget.MediaController.MediaPlayerControl;

public class MulticastAudioPlayer implements MediaPlayer.OnBufferingUpdateListener, MediaPlayer.OnErrorListener,MediaPlayer.OnInfoListener,MediaPlayer.OnPreparedListener ,MediaPlayer.OnCompletionListener{
	
	private MediaPlayer mMeidaPlayer;
	
	public static final String TAG = "AudioPlayer";
	
	Context mContext;
	
	public String mUrl;
	
    public MulticastAudioPlayer(Context context) {
    	mContext = context;
	}
	
	public void play(String url) {
		mMeidaPlayer = new MediaPlayer();

		if (url == null) {
			url = "udp://229.0.0.1:1234";
		}
		
		mUrl = url;
		
		try {	
			mMeidaPlayer.setOnPreparedListener(this);
			mMeidaPlayer.setOnErrorListener(this);
			mMeidaPlayer.setAudioStreamType(AudioManager.STREAM_MUSIC);
			mMeidaPlayer.setOnInfoListener(this);
			mMeidaPlayer.setOnCompletionListener(this);			
//			mMeidaPlayer.setDataSource(mMediaPath + "-" + "audio/" + mMediaBitRates);
			mMeidaPlayer.setDataSource(url);
			mMeidaPlayer.prepareAsync();
			Log.d(TAG, "setDataSource success!");

		} catch (Exception e) {
			Log.e(TAG, e.toString());
			e.printStackTrace();
		}
	}
	
	public void play(FileDescriptor url) {
		mMeidaPlayer = new MediaPlayer(); 
		try {	
			mMeidaPlayer.setOnPreparedListener(this);
			mMeidaPlayer.setOnErrorListener(this);
			mMeidaPlayer.setAudioStreamType(AudioManager.STREAM_MUSIC);

			
//			mMeidaPlayer.setDataSource(mMediaPath + "-" + "audio/" + mMediaBitRates);
			mMeidaPlayer.setDataSource(url);
			mMeidaPlayer.prepareAsync();
			Log.d(TAG, "setDataSource success!");

		} catch (Exception e) {
			Log.e(TAG, e.toString());
			e.printStackTrace();
		}
	}
	
	public void stop() {
		mute(true);
		new Handler().postDelayed(new Runnable() {
			@Override
			public void run() {
				// TODO Auto-generated method stub
				if (mMeidaPlayer != null) {
					mMeidaPlayer.reset();
					mMeidaPlayer.release();
					mMeidaPlayer = null;
					mute(false);
				}
			}
		},1500);
		sendResultToServer(mContext,MulticastVideoPlayer.COMMAND_PLAY_RESULT,"0");
	}
	
    public void sendResultToServer(Context context,String command,String errCode) {
    	if (context == null) {
    		return;
    	}
		Intent i = new Intent();
		i.setAction(command);
		i.putExtra("errcode", errCode);
		context.sendBroadcast(i);
    }

	@Override
	public void onPrepared(MediaPlayer arg0) {
		// TODO Auto-generated method stub
		mMeidaPlayer.start();
		sendResultToServer(mContext,MulticastVideoPlayer.COMMAND_PLAY_RESULT,"0");
	}



	@Override
	public boolean onInfo(MediaPlayer arg0, int arg1, int arg2) {
		// TODO Auto-generated method stub
		if (arg1 == 1001 && arg2 == 0000) {
			if (mMeidaPlayer != null) {
				mMeidaPlayer.reset();
				try {
					mMeidaPlayer.setDataSource(mUrl);
					mMeidaPlayer.prepareAsync();
				} catch (IllegalArgumentException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				} catch (SecurityException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				} catch (IllegalStateException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				} catch (IOException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
				
			}
			
		}
		return false;
	}



	@Override
	public boolean onError(MediaPlayer arg0, int arg1, int arg2) {
		// TODO Auto-generated method stub
		sendResultToServer(mContext,MulticastVideoPlayer.COMMAND_PLAY_RESULT,"playfailed");
		return false;
	}



	@Override
	public void onBufferingUpdate(MediaPlayer arg0, int arg1) {
		// TODO Auto-generated method stub
		
	}

	@Override
	public void onCompletion(MediaPlayer mp) {
		// TODO Auto-generated method stub
		mMeidaPlayer.reset();
	}
	
	public void mute(boolean needMute) {
		if (mContext == null) {
			return;
		}
		AudioManager am = (AudioManager) mContext.getSystemService(Context.AUDIO_SERVICE);
		if (am != null) {
			am.setStreamMute(AudioManager.STREAM_MUSIC, needMute);
		}
	}

}
