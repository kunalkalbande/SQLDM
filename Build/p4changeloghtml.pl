#
# p4changeloghtml.pl
#
# Requires:  Perl (http://www.activestate.com/) and P4Perl (http://public.perforce.com/guest/tony_smith/perforce/API/Perl/index.html)
# Use:  perl p4changeloghtml.pl label1 label2 > changelogoutput.html
# Example:  perl p4changeloghtml.pl sqlcm_3.0.1004.1 sqlcm_3.0.1018.2 > output.html
# The script uses the currently set perforce defaults ("p4 set" to view them).  This includes the user and client workspace.
# 
use P4;
my $p4 = new P4;
my $p4NoParse = new P4 ;

my $productName = "SQL diagnostic manager" ;
my $showDiffs = 'yes' ;  # 'yes' for true, undef for false
my @noDiffList = ("\\.Designer.cs", "\\.resx", "Documentation", "documentation") ;
my $errorsFound = undef ;


$p4->ParseForms() ;

$p4->Connect() or die( "Failed to connect to Perforce Server" );
$p4NoParse->Connect() or die("Failed to connect to Perforce Server" );

my $label1 = shift ;
my $label2 = shift ;
my $tempProductName = shift ;

if($tempProductName)
{
	$productName = $tempProductName ;
}

my $results = $p4->Run("changes", "-l", "//...\@>$label1,\@<=$label2") ;

if($p4->ErrorCount > 0)
{
	print "Error obtaining changelog data:  " . $p4->Errors ;
	exit 1 ;
}

print "<html><head>" ;
print "<script>
function toggleLayer( whichLayer, link )
{
  var elem, vis;
  var linkElem ;
  
  if( document.getElementById ) // this is the way the standards work
  {
    elem = document.getElementById( whichLayer );
    linkElem = document.getElementById( link );
  }
  else if( document.all ) // this is the way old msie versions work
  {
    elem = document.all[whichLayer];
    linkElem = document.all[link];
  }
  else if( document.layers ) // this is the way nn4 works
  {
    elem = document.layers[whichLayer];
    linkElem = document.layers[link];
  }
  vis = elem.style;
  // if the style.display value is blank we try to figure it out here
  if(vis.display==''&&elem.offsetWidth!=undefined&&elem.offsetHeight!=undefined)
    vis.display = (elem.offsetWidth!=0&&elem.offsetHeight!=0)?'block':'none';
  vis.display = (vis.display==''||vis.display=='block')?'none':'block';
  if(vis.display == 'none')
  	linkElem.innerHTML = '+' ;
  else
  	linkElem.innerHTML = '-' ;
}
</script>" ;
print "<title>$productName changes from $label1 to $label2</title></head><body>" ;

print "<h2>$productName changes from $label1 to $label2</h2>" ;

if($results == "")
{
	print "No changes found." ;
}
else 
{
   my $processedResults;
   if(ref($results) eq "HASH")
   {
      # Only one result - push it into a list
      @$processedResults = ();
      unshift(@$processedResults, $results);
   }else
   {
      $processedResults = $results;
   }
   
	for my $item (reverse @$processedResults)
	{
		my %hash = %$item ;
		
		print "<hr><b>Changelist $hash{'change'}</b><br>" ;
		print "Submitted by $hash{'user'} on " ;
		print scalar localtime($hash{'time'}) ;
		print "<br>$hash{'desc'}<br><br>" ;
		
		my $changeListNumber = $hash{'change'} ;
		my $results2 = $p4->fstat("-e",  $changeListNumber, "//...") ;
		if($p4->ErrorCount > 0)
		{
			print "Error obtaining changelist data for $changeListNumber:<br>" . $p4->Errors . "<br>" ;
			$errorsFound = 1 ;
			next ;
		}
		for my $item2 (@$results2)
		{
	  	   my %hash2 = %$item2 ;
			if(length($hash2{'headAction'}))
			{
				my $headRev = $hash2{'headRev'} ;
				my $previousRev = $headRev - 1 ;
				my $fileName = $hash2{'depotFile'} ;
				my $fileKey = strip_slashes("$fileName#$headRev") ;
				
				if(show_diff($fileName) && $showDiffs)
				{
					print "<a id=\"link_$fileKey\" href=\"javascript:toggleLayer('$fileKey', 'link_$fileKey');\">+</a> $hash2{'headAction'}: $fileName#$headRev<br>" ;
					my $results3 = $p4NoParse->diff2("$fileName#$previousRev", "$fileName#$headRev") ;
					print "<div style=\"display: none;\" id=\"$fileKey\"><code>" ;
					if($p4NoParse->ErrorCount > 0)
					{
						print "Error obtaining diff information for $fileName:<br>" . $p4NoParse->Errors ;
						$errorsFound = 1 ;
					}
					else
					{
						for my $item3 (@$results3)
						{
							my @diffLines = split /\n/, $item3 ;
							for my $item4 (@diffLines)
							{
								print escape_for_html($item4) . "<br>" ;
							}
						}
					}
					print "</code></div>" ;
				}
				else
				{
					print "+ $hash2{'headAction'}: $fileName#$headRev<br>" ;
				}
			}
		}	
	}
}

print "</body></html>" ;

$p4->Disconnect();
$p4NoParse->Disconnect() ;

if($errorsFound)
{
	exit 1 ;
}

sub strip_slashes
{
	my $stringVar = shift ;
	$stringVar =~ s/\///g ;
	
	return $stringVar ;
}

sub escape_for_html
{
	my $stringVar = shift ;
	#$stringVar =~ s/\"/&quot;/g ;
	$stringVar =~ s/&/&amp;/g ;
	$stringVar =~ s/</&lt;/g ;
	$stringVar =~ s/>/&gt;/g ;
	$stringVar =~ s/ /&nbsp;/g ;
	
	return $stringVar ;
}

sub show_diff
{
	my $fileName = shift ;
	
	foreach $pattern (@noDiffList)
	{
		if($fileName =~ m/$pattern/)
		{
			return undef ;
		}
	}
	
	return 'yes' ;
}