package net.baviereteam.mcmerchants;

import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.util.concurrent.CompletableFuture;
import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import net.baviereteam.mcmerchants.json.ResponseRoot;

public class McMerchantsService {
	private final String apiUrl;

	public McMerchantsService(String baseUrl) {
		this.apiUrl = baseUrl.strip() + "/api/stock/%s?synthetic=true";
	}
	
	public CompletableFuture<QueryResult> query(String item) {
		return executeRequest(item)
				.thenApply(this::craftResult);
	}

	private CompletableFuture<String> executeRequest(String arg) {
		HttpClient client = HttpClient.newHttpClient();
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
		} catch (JsonProcessingException e) {
			result = new QueryResult(e);
		}

		return result;
	}

	private ResponseRoot deserializeResult(String input) throws JsonProcessingException {
		ObjectMapper mapper = new ObjectMapper();
		return mapper.readValue(input, ResponseRoot.class);
	}
}