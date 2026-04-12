package net.baviereteam.mcmerchantschatplugin;

import com.google.gson.JsonSyntaxException;
import io.papermc.paper.plugin.lifecycle.event.types.LifecycleEvents;
import net.kyori.adventure.text.Component;
import net.kyori.adventure.text.minimessage.MiniMessage;
import org.bukkit.Bukkit;
import org.bukkit.entity.Entity;
import org.bukkit.plugin.java.JavaPlugin;

import javax.net.ssl.SSLHandshakeException;
import java.util.concurrent.CompletionException;

public class McMerchantsChatPlugin extends JavaPlugin {
	private McmCommand mcmCommand;
	private BomCommand bomCommand;

	@Override
	public void onEnable() {
		saveDefaultConfig();

		String mcMerchantsUrl = null;
		try {
			mcMerchantsUrl = this.getConfig().get("mcmerchants_url").toString();

		} catch (NullPointerException e) {
			getComponentLogger().error(Component.text("Could not register McMerchants URL. Make sure it is correctly set in your configuration file."));
			return;
		}

		mcmCommand = new McmCommand(this, mcMerchantsUrl);
		this.getLifecycleManager().registerEventHandler(
				LifecycleEvents.COMMANDS,
				event -> event.registrar().register(McmCommand.getMcmCommand(mcmCommand))
		);

		if (Bukkit.getServer().getPluginManager().getPlugin("FastAsyncWorldEdit") == null) {
			getComponentLogger().info("FastAsyncWorldEdit is not installed on this server. The /bom command will not be available.");
		} else {
			bomCommand = new BomCommand(this, mcMerchantsUrl);
			this.getLifecycleManager().registerEventHandler(
					LifecycleEvents.COMMANDS,
					event -> event.registrar().register(BomCommand.getBomCommand(bomCommand))
			);
		}
	}

	public void sendFailAnswer(String message, Entity executor) {
		if (executor != null) {
			executor.sendMessage(MiniMessage.miniMessage().deserialize(
					message + "\n<red><b>Please tell someone!</b></red>"
			));
		}
	}

	public void sendSuccessAnswer(Component message, Entity executor) {
		if (executor != null) {
			executor.sendMessage(message);
		}
	}

	public void broadcastSuccessAnswer(Component answer) {
		getServer().sendMessage(answer);
	}

	public void handleError(Throwable err, Entity executor) {
		getComponentLogger().error(err.getMessage());

		// Anything's a CompletionException if it happens during some async operations
		if (err instanceof CompletionException) {
			err = err.getCause();
		}

		String messageToPlayer;

		if (err instanceof SSLHandshakeException && err.getMessage().contains("PKIX path building failed")) {
			getComponentLogger().warn("If the keystore has been updated since the server started, you might need to restart Minecraft.");
			messageToPlayer = "I can't talk to McMerchants in a secure way :(";

		} else if (err instanceof JsonSyntaxException) {
			messageToPlayer = "I can't understand McMerchants' answer :(";

		} else {
			messageToPlayer = "I don't know why that didn't work :(";
		}

		sendFailAnswer(messageToPlayer, executor);
	}
}