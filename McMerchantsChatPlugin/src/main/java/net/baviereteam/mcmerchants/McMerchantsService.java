package net.baviereteam.mcmerchants;

import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.util.concurrent.CompletableFuture;
import com.google.gson.Gson;
import com.google.gson.JsonSyntaxException;
import net.baviereteam.mcmerchants.json.ResponseRoot;

public class McMerchantsService {
	private final String apiUrl;
	private final HttpClient client;

	public McMerchantsService(String baseUrl) {
		this.apiUrl = baseUrl.strip() + "/api/stock/%s?synthetic=true";
		this.client = HttpClient.newHttpClient();
	}
		
	public CompletableFuture<QueryResult> query(String item) {
		return executeRequest(item)
				.thenApply(this::craftResult);
	}

	private CompletableFuture<String> executeRequest(String arg) {
		HttpRequest request = HttpRequest.newBuilder()
				.uri(URI.create(apiUrl.formatted(arg)))
				.build();

		// i'd like this to return an async but when it resolves, it has the parsed string
		return client.sendAsync(request, HttpResponse.BodyHandlers.ofString())
				.thenApply(HttpResponse::body);
	}

	private QueryResult craftResult(String json) {
		QueryResult result;

		try {
			result = new QueryResult(deserializeResult(json));
		} catch (JsonSyntaxException e) {
			result = new QueryResult(e);
		}

		return result;
	}

	private ResponseRoot deserializeResult(String input) throws JsonSyntaxException {
		Gson gson = new Gson();
		return gson.fromJson(input, ResponseRoot.class);
	}
}