package net.baviereteam.mcmerchantschatplugin;

import com.mojang.brigadier.tree.LiteralCommandNode;
import io.papermc.paper.command.brigadier.CommandSourceStack;
import io.papermc.paper.command.brigadier.Commands;
import io.papermc.paper.command.brigadier.argument.ArgumentTypes;
import io.papermc.paper.registry.RegistryKey;

public class McmCommandBuilder {
    public static LiteralCommandNode<CommandSourceStack> getMcmCommand(McMerchantsChatPlugin plugin) {
        return Commands.literal("mcm")
            .then(Commands
                .argument("item", ArgumentTypes.resource(RegistryKey.ITEM))
                .executes(plugin::onCommand)
            )
            .build();
    }
}
