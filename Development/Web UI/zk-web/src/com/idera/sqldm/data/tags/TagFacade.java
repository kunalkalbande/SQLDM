package com.idera.sqldm.data.tags;

import java.util.Comparator;
import java.util.LinkedList;
import java.util.List;

import com.idera.common.rest.RestException;
import com.idera.sqldm.i18n.SQLdmI18NStrings;
import com.idera.sqldm.rest.SQLDMRestClient;

public class TagFacade {

	public static List<Tag> getTags(String productInstanceName) throws TagException {
		try
		{
			return SQLDMRestClient.getInstance().getTags(productInstanceName);
		}
		catch (RestException x)
		{
			throw new TagException(x, SQLdmI18NStrings.FAILED_TO_GET_DASHBOARD_TAGS);
		}
	}
	public static List<Tag> getDummyTags() throws TagException {
		try
		{
			List<Tag> tags = new LinkedList<>();
			Tag tag = new Tag();
			tag.setId(1);
			tag.setName("Development");
			tag.setCount(1);
			tag.setMaxSeverity(1);
			tags.add(tag);
			tag = new Tag();
			tag.setId(2);
			tag.setName("Staging");
			tag.setCount(1);
			tag.setMaxSeverity(4);

			tags.add(tag);
			tag.setId(3);
			tag.setName("Production");
			tag.setCount(1);
			tag.setMaxSeverity(8);

			tags.add(tag);

			return tags;
		}
		catch (Exception x)
		{
			throw new TagException(x, SQLdmI18NStrings.FAILED_TO_GET_DASHBOARD_TAGS);
		}
	}

	public static final Comparator<Tag> TAG_NAME_STRING_COMPARATOR_ASC = new Comparator<Tag>() {
		@Override
		public int compare(Tag t1, Tag t2) {

			return compareName(t1, t2, true);		
		}
	};
			
					
	public static final Comparator<Tag> TAG_NAME_STRING_COMPARATOR_DESC = new Comparator<Tag>() {
		@Override
		public int compare(Tag t1, Tag t2) {

			return compareName(t1, t2, false);		
		}
	};
							
	private static int compareName(Tag t1, Tag t2, boolean asc) {
		int ret = 0;

		if(t1 != null && t2 != null && t1.getName() != null && t2.getName() != null) {
										
			ret = t1.getName().toLowerCase().compareTo(t2.getName().toLowerCase());
		}
		
		return (ret  * (asc ? 1 : -1));
	}
	
}
