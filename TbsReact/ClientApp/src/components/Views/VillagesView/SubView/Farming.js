import React from "react";
import { Grid } from "@mui/material";
import ContentBox from "../../../ContentBox";

const Farming = () => {
	return (
		<ContentBox>
			<Grid container spacing={2}>
				<Grid item xs={6}>
					<Grid container spacing={2}>
						<Grid item xs={12}>
							<ContentBox name="Name list" />
						</Grid>
						<Grid item xs={12}>
							<ContentBox name="List target" />
						</Grid>
						<Grid item xs={6}>
							<ContentBox name="New list" />
						</Grid>
						<Grid item xs={6}>
							<ContentBox name="Delete list" />
						</Grid>
						<Grid item xs={12}>
							<ContentBox name="ATTACK TARGET in list" />
						</Grid>
					</Grid>
				</Grid>
				<Grid item xs={6}>
					<Grid container spacing={2}>
						<Grid item xs={12}>
							<ContentBox name="Add target" />
						</Grid>
						<Grid item xs={12}>
							<ContentBox name="Update target" />
						</Grid>
						<Grid item xs={12}>
							<ContentBox name="Delete target" />
						</Grid>
						<Grid item xs={12}>
							<ContentBox name="Clear all target" />
						</Grid>
						<Grid item xs={12}>
							<ContentBox name="Search inactive target" />
						</Grid>
					</Grid>
				</Grid>
				<Grid item xs={12}>
					<ContentBox name="WARNING: USE AT YOUR OWN RISK" />
				</Grid>
			</Grid>
		</ContentBox>
	);
};

export default Farming;
