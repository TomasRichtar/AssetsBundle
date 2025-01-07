<?php
​
// Allow from any origin
if (isset($_SERVER['HTTP_ORIGIN'])) {
    // Decide if the origin in $_SERVER['HTTP_ORIGIN'] is one
    // you want to allow, and if so:
    header("Access-Control-Allow-Origin: {$_SERVER['HTTP_ORIGIN']}");
    header('Access-Control-Allow-Credentials: true');
    header('Access-Control-Max-Age: 86400');    // cache for 1 day
}
​
$ftp_username = "tastyair.cz";
$ftp_userpass = "3uqUrQ8wdDGT";
$ftp_server = "tastyair.cz";
$ftp_dir = htmlspecialchars($_POST["ftpdir"]);
​
$ftp_connection = ftp_connect($ftp_server);
 
if($ftp_connection) {
    $login = ftp_login($ftp_connection, $ftp_username, $ftp_userpass);
     
    if($login) {
        $file_list = ftp_nlist($ftp_connection, $ftp_dir);
        sort($file_list);
        foreach($file_list as $key=>$dat) {
            echo $dat."\n";
       }
    }
    ftp_close($ftp_connection);
}
?>	