<?php
  $pdo = new PDO('sqlite:Taiwan_2025_03_27_gis.db');

  

  $SQL = "
	SELECT
	  *
	FROM
	  `tiles`
  ";
  $stmt = $pdo->prepare($SQL);
  $stmt->execute();
  $ra = $stmt->fetchAll(PDO::FETCH_ASSOC);

  // 用交易模式加速
  $pdo->beginTransaction();


  for($i=0,$max_i=count($ra);$i<$max_i;$i++){
	  // 用 deflate 壓縮 再寫回
	  $ra[$i]['tile_data'] = gzdeflate($ra[$i]['tile_data'],9);
	  $SQL = "
		UPDATE
		  `tiles`
		SET
		  `tile_data` = :tile_data
		WHERE
		1=1
		AND `zoom_level` = :zoom_level
		AND `tile_column` = :tile_column
		AND `tile_row` = :tile_row
	  ";
	  $stmt = $pdo->prepare($SQL);
	  $stmt->execute(array(
		  ':tile_data' => $ra[$i]['tile_data'],
		  ':zoom_level' => $ra[$i]['zoom_level'],
		  ':tile_column' => $ra[$i]['tile_column'],
		  ':tile_row' => $ra[$i]['tile_row']
		));
	echo sprintf("%d / %d ...\n",$i+1,$max_i);
	if($i % 10000==0) {
		$pdo->commit();
		$pdo->beginTransaction();
	}
  }
  $pdo->commit();

  // 最後再 vacuum
  $SQL = "
	VACUUM
  ";
  $stmt = $pdo->prepare($SQL);
  $stmt->execute();

  echo "Done...\n";