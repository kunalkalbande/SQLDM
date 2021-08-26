package com.idera.sqldm.rewrite.provider;

import javax.servlet.ServletContext;

import org.apache.log4j.Logger;
import org.ocpsoft.logging.Logger.Level;
import org.ocpsoft.rewrite.annotation.RewriteConfiguration;
import org.ocpsoft.rewrite.config.Configuration;
import org.ocpsoft.rewrite.config.ConfigurationBuilder;
import org.ocpsoft.rewrite.config.Direction;
import org.ocpsoft.rewrite.config.Log;
import org.ocpsoft.rewrite.servlet.config.Forward;
import org.ocpsoft.rewrite.servlet.config.HttpConfigurationProvider;
import org.ocpsoft.rewrite.servlet.config.Path;
import org.ocpsoft.rewrite.servlet.config.Substitute;


@RewriteConfiguration
public class RewriteConfigurationProvider extends HttpConfigurationProvider
{

    private static final Logger logger = Logger.getLogger(RewriteConfigurationProvider.class);


    @Override
    public Configuration getConfiguration(ServletContext context)
    {
        /*
         * Configures and returns all the rules based on installed products. It
         * takes into consideration the registered default home page and
         * subsequent pages.
         */
        ConfigurationBuilder configurationBuilder = ConfigurationBuilder.begin();
        
        configurationBuilder.addRule().when(Direction.isInbound().and(Path.matches("/sqldm/{instance}/home")))
        .perform(Log.message(Level.INFO, "Applying inbound rewrite filter from SQLDM")
        .and(Forward.to("/~./sqldm/home.zul?id=-1&instance={instance}")));
        
        configurationBuilder.addRule().when(Direction.isOutbound().and(Path.matches("/sqldm/{instance}/dashboard")))
        .perform(Log.message(Level.INFO, "Applying outbound rewrite filter from SQLDM")
        .and(Substitute.with("/sqldm/{instance}/home")));
        
        // /alerts ---> /alerts.zul?id=-1
        configurationBuilder.addRule().when(Direction.isInbound().and(Path.matches("/sqldm/{instance}/alerts")))
                        .perform(Log.message(Level.INFO, "Applying inbound rewrite filter from SQLDM")
                        .and(Forward.to("/~./sqldm/alerts.zul?id=-1&instance={instance}")));

        // /topten ---> /topten.zul?id=-1
        configurationBuilder.addRule().when(Direction.isInbound().and(Path.matches("/sqldm/{instance}/topten")))
                        .perform(Log.message(Level.INFO, "Applying inbound rewrite filter from SQLDM").and(Forward.to("/~./sqldm/topten.zul?id=-1&instance={instance}")));

        // /singleInstance/([0-9]+) ---> /singleInstance.zul?id=$1
        configurationBuilder.addRule().when(Direction.isInbound().and(Path.matches("/sqldm/{instance}/singleInstance/{id}")))
                        .perform(Log.message(Level.INFO, "Applying inbound rewrite filter from SQLDM for Single Instance with ID").and(Forward.to("/~./sqldm/singleInstance.zul?id={id}&instance={instance}"))).where("id").matches("[0-9]+");

        configurationBuilder.addRule().when(Direction.isInbound().and(Path.matches("/sqldm/{instance}/singleInstance/instanceName/{domainname}/{instancenameswa}")))
		.perform(Log.message(Level.INFO, "Applying inbound rewrite filter from SQLDM for Single Instance with ID").and(Forward.to("/~./sqldm/singleInstance.zul?domainname={domainname}&&instance={instance}&&instancenameswa={instancenameswa}")));
        
        configurationBuilder.addRule().when(Direction.isInbound().and(Path.matches("/sqldm/{instance}/singleInstance/instanceName/{name}")))
        				.perform(Log.message(Level.INFO, "Applying inbound rewrite filter from SQLDM for Single Instance with ID").and(Forward.to("/~./sqldm/singleInstance.zul?name={name}&&instance={instance}")));
        
        
        
        configurationBuilder.addRule().when(Direction.isInbound()
	        	.and(Path.matches("/sqldm/{instance}/{filepath}")))
	        	.perform(Log.message(Level.INFO, "Applying inbound rewrite filter from SQLDM to filepath")
	        	.and(Forward.to("/~./sqldm/{filepath}.zul"))).where("filepath");
        
        configurationBuilder.addRule().when(Direction.isInbound()
            	.and(Path.matches("/sqldm/{instance}/images/{filepath}")))
            	.perform(Log.message(Level.INFO, "Applying inbound rewrite filter from SQLDM to filepath")
            	.and(Forward.to("/~./sqldm/images/{filepath}"))).where("filepath");
        
        configurationBuilder.addRule().when(Direction.isInbound()
            	.and(Path.matches("/sqldm/{instance}/~./sqldm/com/idera/sqldm/images/{filepath}")))
            	.perform(Log.message(Level.INFO, "Applying inbound rewrite filter from SQLDM to filepath")
            	.and(Forward.to("/~./sqldm/com/idera/sqldm/images/{filepath}"))).where("filepath");

        configurationBuilder.addRule().when(Direction.isInbound()
            	.and(Path.matches("/sqldm/{instance}/css/{filepath}")))
            	.perform(Log.message(Level.INFO, "Applying inbound rewrite filter from SQLDM to filepath")
            	.and(Forward.to("/~./sqldm/css/{filepath}"))).where("filepath");
        
        configurationBuilder.addRule().when(Direction.isInbound()
            	.and(Path.matches("/sqldm/{instance}/js/{filepath}")))
            	.perform(Log.message(Level.INFO, "Applying inbound rewrite filter from SQLDM to filepath")
            	.and(Forward.to("/~./sqldm/js/{filepath}"))).where("filepath");

        return configurationBuilder;
    }


    @Override
    public int priority()
    {
        // Returns the priority for the same.
        return 0;
    }

}
