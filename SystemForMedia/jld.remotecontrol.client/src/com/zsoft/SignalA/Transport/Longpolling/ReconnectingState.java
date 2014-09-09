package com.zsoft.SignalA.Transport.Longpolling;

import org.json.JSONObject;

import android.os.Handler;

import com.turbomanage.httpclient.AsyncCallback;
import com.turbomanage.httpclient.HttpResponse;
import com.zsoft.SignalA.ConnectionBase;
import com.zsoft.SignalA.ConnectionState;
import com.zsoft.SignalA.SignalAUtils;
import com.zsoft.SignalA.SendCallback;
import com.zsoft.SignalA.Transport.ProcessResult;
import com.zsoft.SignalA.Transport.TransportHelper;
import com.zsoft.parallelhttpclient.ParallelHttpClient;

public class ReconnectingState extends StopableStateWithCallback {

	public ReconnectingState(ConnectionBase connection) {
		super(connection);
	}

	@Override
	public ConnectionState getState() {
		return ConnectionState.Reconnecting;
	}

	@Override
	public void Start() {
	}

	@Override
	public void Send(CharSequence text, SendCallback callback) {
		callback.OnError(new Exception("Not connected"));
	}

	@Override
	protected void OnRun() {
		if(DoStop()) return; 

	    if (mConnection.getMessageId() == null)
		{
	    	// No message received yet....connect instead of reconnect
			mConnection.SetNewState(new ConnectingState(mConnection));
			return;
		}
	    
	    String url = SignalAUtils.EnsureEndsWith(mConnection.getUrl(), "/");
		url += "reconnect";
	    url += TransportHelper.GetReceiveQueryString(mConnection, null, TRANSPORT_NAME);

		AsyncCallback cb = new AsyncCallback() {
			
			@Override
			public void onComplete(HttpResponse httpResponse) {
				if(DoStop()) return; 

                try
                {
                	if(httpResponse.getStatus()==200)
                	{
                		JSONObject json = JSONHelper.ToJSONObject(httpResponse.getBodyAsString());
	                    if (json!=null)
	                    {
	                		ProcessResult result = TransportHelper.ProcessResponse(mConnection, json);
	
	                		if(result.processingFailed)
	                		{
	                    		mConnection.setError(new Exception("Error while proccessing response."));
	                		}
	                		else if(result.disconnected)
	                		{
	      						mConnection.SetNewState(new DisconnectedState(mConnection));
	    						return;
	                		}
	                    }
	                    else
	                    {
						    mConnection.setError(new Exception("Error when parsing response to JSONObject."));
	                    }
                    }
                    else
                    {
					    mConnection.setError(new Exception("Error when calling endpoint. Returncode: " + httpResponse.getStatus()));
                    }
                }
                finally
                {
					if(mConnection.getCurrentState() == ReconnectingState.this)
					{
						// Delay before reconnecting
						Delay(2000, new DelayCallback() {
							
							@Override
							public void OnStopedBeforeElapsed() {
								mIsRunning.set(false);
								mConnection.SetNewState(new DisconnectedState(mConnection));
							}
							
							@Override
							public void OnDelayElapsed() {
								mIsRunning.set(false);
								// Loop if we are still reconnecting
								Run();
							}
						});
					}
                }
			}
           @Override
            public void onError(Exception ex) {
        		mConnection.SetNewState(new DisconnectedState(mConnection));
			    mConnection.setError(ex);
				
			}
		};

		
		synchronized (mCallbackLock) {
			//mCurrentCallback = cb;
		}

		ParallelHttpClient httpClient = new ParallelHttpClient();
        httpClient.setMaxRetries(1);
        httpClient.post(url, null, cb);
	}

	protected void Delay(final long milliSeconds, final DelayCallback cb) {
		final long startTime = System.currentTimeMillis();
		final Handler handler = new Handler();
		final Runnable runnable = new Runnable() {
		  @Override
		  public void run() {
			  if(DoStop()) {
				  cb.OnStopedBeforeElapsed();
			  }
			  else {
				  long difference = System.currentTimeMillis() - startTime;
				  if(difference < milliSeconds) {
					  handler.postDelayed(this, 500);
				  }
				  else {
					  	cb.OnDelayElapsed();
				  }
			  }
				  
		  }
		};
		
		handler.postDelayed(runnable, 500);
	}

	private abstract class DelayCallback {
		public abstract void OnDelayElapsed();
		public abstract void OnStopedBeforeElapsed();
	}
	
}
