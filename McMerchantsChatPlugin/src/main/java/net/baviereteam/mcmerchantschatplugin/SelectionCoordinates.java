package net.baviereteam.mcmerchantschatplugin;

import com.sk89q.worldedit.math.BlockVector3;
import com.sk89q.worldedit.regions.Region;

import java.util.Objects;

public class SelectionCoordinates {
	public String dimension;
	public int startX, startY, startZ, endX, endY, endZ;
	
	public SelectionCoordinates(Region region) {
		dimension = Objects.requireNonNull(region.getWorld()).getName();
		
		BlockVector3 startPoint = region.getMinimumPoint();
		BlockVector3 endPoint = region.getMaximumPoint();
		
		startX = startPoint.x();
		startY = startPoint.y();
		startZ = startPoint.z();
		endX = endPoint.x();
		endY = endPoint.y();
		endZ = endPoint.z();
	}
}
