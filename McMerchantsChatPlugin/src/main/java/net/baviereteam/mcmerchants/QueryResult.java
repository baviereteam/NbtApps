package net.baviereteam.mcmerchants;

import net.baviereteam.mcmerchants.json.ResponseRoot;

public class QueryResult {
	private ResponseRoot response;
	private Throwable error;
	
	public QueryResult(ResponseRoot response) {
		this.response = response;
	}
	public QueryResult(Throwable error) {
		this.error = error;
	}
	
	public boolean hasError() {
		return this.error != null;
	}
	public Throwable getError() {
		return this.error;
	}
	public ResponseRoot getResponse() {
		return this.response;
	}
}
