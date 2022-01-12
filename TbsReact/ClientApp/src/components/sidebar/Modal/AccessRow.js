import PropTypes from "prop-types";
import React from "react";
import { TableCell, TableRow } from "@mui/material";

const AccessRow = ({ accesses, handler, selected }) => {
	const rows = accesses.map((access) => (
		<TableRow
			hover
			onClick={() => handler(access)}
			selected={access.id === selected}
			key={access.id}
		>
			<TableCell>
				{[access.proxy.ip, ":", access.proxy.port].join("")}
			</TableCell>
			<TableCell>{access.proxy.username}</TableCell>
			<TableCell>{access.proxy.OK === true ? "✔" : "❌"}</TableCell>
		</TableRow>
	));
	return <>{rows}</>;
};

AccessRow.propTypes = {
	accesses: PropTypes.array.isRequired,
	handler: PropTypes.func.isRequired,
	selected: PropTypes.number.isRequired,
};

export default AccessRow;
