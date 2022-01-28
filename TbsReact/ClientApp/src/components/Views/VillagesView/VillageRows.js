import PropTypes from "prop-types";
import React from "react";
import { TableCell, TableRow } from "@mui/material";

const VillageRows = ({ villages, handler, selected }) => {
	const rows = villages.map((village) => (
		<TableRow
			hover
			onClick={() => handler(village)}
			selected={village.id === selected}
			key={village.id}
		>
			<TableCell>{village.name}</TableCell>
			<TableCell>{`${village.coords.x}/${village.coords.y}`}</TableCell>
		</TableRow>
	));
	return <>{rows}</>;
};

VillageRows.propTypes = {
	villages: PropTypes.array.isRequired,
	handler: PropTypes.func.isRequired,
	selected: PropTypes.number.isRequired,
};

export default VillageRows;
