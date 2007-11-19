begin
{
	$out = ""
}
process
{
	if($_) {
		$codes = $_.Split("`0")
		foreach($code in $codes) {
			$out += "`"$code`", "
		}	
	} else {
		Write-Error "USAGE: ppp zombie 70 140 | Format-Ppp"
	}
}
end
{
	if($out) {
		$out
	}
}