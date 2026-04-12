package net.baviereteam.mcmerchantschatplugin;

import com.mojang.brigadier.Command;
import com.mojang.brigadier.arguments.LongArgumentType;
import com.mojang.brigadier.context.CommandContext;
import com.mojang.brigadier.tree.LiteralCommandNode;
import com.sk89q.worldedit.IncompleteRegionException;
import com.sk89q.worldedit.LocalSession;
import com.sk89q.worldedit.WorldEdit;
import com.sk89q.worldedit.bukkit.BukkitAdapter;
import com.sk89q.worldedit.session.SessionManager;
import com.sk89q.worldedit.world.World;
import io.papermc.paper.command.brigadier.CommandSourceStack;
import io.papermc.paper.command.brigadier.Commands;
import net.kyori.adventure.text.Component;
import net.kyori.adventure.text.minimessage.MiniMessage;
import org.bukkit.entity.Player;
import com.sk89q.worldedit.regions.Region;
import org.bukkit.inventory.ItemType;

public class BomCommand {
	private final McMerchantsChatPlugin plugin;
	private final String mcMerchantsUrl;

	public BomCommand(McMerchantsChatPlugin plugin, String mcMerchantsUrl) {
		this.plugin = plugin;
		this.mcMerchantsUrl = mcMerchantsUrl;
	}

	public static LiteralCommandNode<CommandSourceStack> getBomCommand(BomCommand commandClass) {
		return Commands.literal("bom")
				.then(Commands
						.argument("bomId", LongArgumentType.longArg(1))
						.executes(commandClass::onBomCommand)
				)
				.build();
	}

	public int onBomCommand(CommandContext<CommandSourceStack> context) {
		final org.bukkit.entity.Entity executor = context.getSource().getExecutor();
		if (!(executor instanceof Player bukkitPlayer)) {
			plugin.sendSuccessAnswer(
					MiniMessage.miniMessage().deserialize("Only players can use this command."),
					executor
			);
			return Command.SINGLE_SUCCESS;
		}

		final long bomId = context.getArgument("bomId", Long.class);
		
		final com.sk89q.worldedit.entity.Player wePlayer = BukkitAdapter.adapt(bukkitPlayer);
		final SelectionCoordinates coordinates = GetSelectionBounds(wePlayer);
		
		if (coordinates == null) {
			plugin.sendSuccessAnswer(
					MiniMessage.miniMessage().deserialize("Please create a WorldEdit selection first."),
					executor
			);
			return Command.SINGLE_SUCCESS;
		}
		
		Component answer = buildLink(bomId, coordinates);
		plugin.sendSuccessAnswer(answer, executor);
		return Command.SINGLE_SUCCESS;
	}
	
	private SelectionCoordinates GetSelectionBounds(com.sk89q.worldedit.entity.Player player) {
		SessionManager manager = WorldEdit.getInstance().getSessionManager();
		LocalSession session = manager.get(player);

		
		World selectionWorld = session.getSelectionWorld();
		if (selectionWorld == null) {
			return null;
		}

		try {
			Region region = session.getSelection(selectionWorld);
			return new SelectionCoordinates(region);
			
		} catch (IncompleteRegionException e) {
			plugin.getComponentLogger().info("The WorldEdit selection was incomplete.");
			return null;
			
		} catch (Exception e) {
			plugin.getComponentLogger().error("Could not obtain selection", e);
			return null;
		}
	}
	
	private Component buildLink(long bomId, SelectionCoordinates selection) {
		StringBuilder sb = new StringBuilder(mcMerchantsUrl);
		sb
				.append("/Boms/")
				.append(bomId)
				.append("?dimension=")
				.append(selection.dimension)
				.append("&startx=")
				.append(selection.startX)
				.append("&starty=")
				.append(selection.startY)
				.append("&startz=")
				.append(selection.startZ)
				.append("&endx=")
				.append(selection.endX)
				.append("&endy=")
				.append(selection.endY)
				.append("&endz=")
				.append(selection.endZ);
		
		String url = sb.toString();
		
		sb = new StringBuilder();
		sb
				.append("Click <b><yellow><click:open_url:'")
				.append(url)
				.append("'>here</click></yellow></b> to open your BOM in the browser.");
		
		return MiniMessage.miniMessage().deserialize(sb.toString());
	}
}
