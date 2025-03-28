<?php
  $pdo = new PDO('sqlite:Taiwan_2025_03_27_gis.db');

  // 建立一個 zip
  $zip = new ZipArchive();
  $zip->open('Taiwan_2025_03_27_gis.zip', ZipArchive::CREATE);
  

  

  $SQL = "
	SELECT
	  *
	FROM
	  `tiles`
  ";
  $stmt = $pdo->prepare($SQL);
  $stmt->execute();
  $ra = $stmt->fetchAll(PDO::FETCH_ASSOC);

  for($i=0,$max_i=count($ra);$i<$max_i;$i++){
	  // 用 deflate 壓縮 再寫回
	  $ra[$i]['tile_data'] = $ra[$i]['tile_data'];
	  $filename = sprintf("tiles/%d/%d/%d.pbf",$ra[$i]['zoom_level'],$ra[$i]['tile_column'],$ra[$i]['tile_row']);
	  $zip->addFromString($filename,$ra[$i]['tile_data']);
	  //進度
	  echo sprintf("%d / %d ...\n",$i+1,$max_i);
  }
  $zip->close();
  echo "Done...\n";
