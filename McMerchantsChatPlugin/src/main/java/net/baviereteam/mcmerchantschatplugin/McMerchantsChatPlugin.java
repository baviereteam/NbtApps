package net.baviereteam.mcmerchantschatplugin;

import com.mojang.brigadier.Command;
import com.mojang.brigadier.context.CommandContext;
import io.papermc.paper.command.brigadier.CommandSourceStack;
import io.papermc.paper.plugin.lifecycle.event.types.LifecycleEvents;
import net.baviereteam.mcmerchants.McMerchantsService;
import net.baviereteam.mcmerchants.QueryResult;
import net.baviereteam.mcmerchants.json.Alley;
import net.baviereteam.mcmerchants.json.FactoryResult;
import net.baviereteam.mcmerchants.json.ResponseRoot;
import net.baviereteam.mcmerchants.json.Trader;
import net.kyori.adventure.text.Component;
import net.kyori.adventure.text.minimessage.MiniMessage;
import org.apache.commons.lang3.ObjectUtils;
import org.bukkit.entity.Entity;
import org.bukkit.inventory.ItemType;
import org.bukkit.plugin.java.JavaPlugin;

import java.util.List;
import java.util.concurrent.CompletableFuture;
import java.util.concurrent.atomic.AtomicBoolean;

public class McMerchantsChatPlugin extends JavaPlugin {
	public McMerchantsService mcMerchantsClient;

	public McMerchantsChatPlugin() {
		
	}

	@Override
	public void onEnable() {
		saveDefaultConfig();
		
		try {
			mcMerchantsClient = new McMerchantsService(this.getConfig().get("mcmerchants_url").toString());
			this.getLifecycleManager().registerEventHandler(
					LifecycleEvents.COMMANDS,
					event -> event.registrar().register(McmCommandBuilder.getMcmCommand(this))
			);
			
		} catch (NullPointerException e) {
			getComponentLogger().error(Component.text("Could not register McMerchants URL. Make sure it is correctly set in your configuration file."));
		}
	}

	public int onCommand(CommandContext<CommandSourceStack> context) {
		final org.bukkit.entity.Entity executor = context.getSource().getExecutor();
		final ItemType itemType = context.getArgument("item", ItemType.class);
		queryMcMerchantsAndShowResults(executor, itemType);
		return Command.SINGLE_SUCCESS;
	}

	public void logFailure(String text) {
		this.getComponentLogger().warn(Component.text(text));
	}

	public void queryMcMerchantsAndShowResults(Entity executor, ItemType itemType) {
		CompletableFuture<QueryResult> futureResult = mcMerchantsClient.query(itemType.getKey().toString());
		
		futureResult.handle((result, err) -> {
			if (err != null) {
				logFailure(err.getMessage());
				sendFailAnswer(executor);
			}

			if (result.hasError()) {
				logFailure(result.getError().toString());
				sendFailAnswer(executor);

			} else {
				ResponseRoot response = result.getResponse();
				Component answer = craftSuccessAnswer(itemType.getKey().toString(), response);
				broadcastSuccessAnswer(answer);
			}
			
			return 0;
		});
	}

	public void sendFailAnswer(Entity executor) {
		if (executor != null) {
			executor.sendMessage("McMerchants did not answer properly :(");
		}
	}
	public void broadcastSuccessAnswer(Component answer) {
		getServer().sendMessage(answer);
	}

	public Component craftSuccessAnswer(String searchedItemName, ResponseRoot response) {
		AtomicBoolean foundAtLeastOnce = new AtomicBoolean(false);

		StringBuilder answer = new StringBuilder();
		answer
				.append("<u><b>McMerchants request for <yellow>")
				.append(searchedItemName)
				.append("</yellow></b></u>\n");

		if (!response.complete) {
			answer.append("<red>Some chunks could not be read. The results displayed here might be incomplete.</red>\n");
		}
		
		// this lists all the existing stores even if they don't carry it
		if (!response.stores.isEmpty()) {
			answer.append("> Stores:\n");
			response.stores.forEach(store -> {
				int total = store.alleys.stream().reduce(
						0,
						(accumulator, alley) -> accumulator + alley.count,
						Integer::sum);
				total += store.bulk;

				if (total == 0) {
					answer
							.append("  - <b>none</b> in <light_purple>")
							.append(store.name)
							.append("</light_purple>");
					
				} else {
					String alleyNames = store.alleys.stream()
							.filter(alley -> alley.count > 0)
							.reduce(
									"",
									(accumulator, alley) -> {
										return accumulator + alley.name + " ";
									},
									String::concat
							);
					
					if (store.bulk > 0) {
						alleyNames += "bulk";
					}
					
					answer
						.append("  - <b>")
						.append(total)
						.append("</b> in <light_purple>")
						.append(store.name)
						.append("</light_purple> (")
						.append(alleyNames.trim())
						.append(")");
				}
				
				answer.append("\n");
			});
		}

		// This only lists factories that produce it
		answer.append("> Factories: ");
		if (response.factories.isEmpty()) {
			answer.append("<b>none</b> producing this item\n");
			
		} else {
			answer.append("\n");
			
			response.factories.forEach(factory -> {
				if (factory.count == 0) {
					answer
							.append("  - <b>none</b> in <light_purple>")
							.append(factory.name)
							.append("</light_purple>\n");
					
				} else {
					answer
							.append("  - <b>")
							.append(factory.count)
							.append("</b> in <light_purple>")
							.append(factory.name)
							.append("</light_purple>\n");
				}
			});
		}

		// this is a list of all trading zones even if they don't have a trade for this
		if (!response.traders.isEmpty()) {
			answer.append("> Trading zones: ");
			List<Trader> tradingZonesWithThisItem =
					response.traders
							.stream()
							.filter(
									trader -> trader.count > 0)
							.toList();

			if (tradingZonesWithThisItem.isEmpty()) {
				answer.append("<b>none</b> with this item\n");
				
			} else {
				answer.append("\n");
				
				response.traders.forEach(tradingZone -> {
					if (tradingZone.count > 0) {
						answer
								.append("  - <b>")
								.append(tradingZone.count)
								.append("</b> trades in <light_purple>")
								.append(tradingZone.name)
								.append("</light_purple>\n");
					}
				});
			}
		}

		return MiniMessage.miniMessage().deserialize(answer.toString());
	}
}