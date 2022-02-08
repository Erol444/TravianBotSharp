import React from "react";
import { Grid } from "@mui/material";
import ContentBox from "../../ContentBox";

const Hero = () => {
	return (
		<ContentBox>
			<Grid container spacing={2}>
				<Grid item xs={4}>
					<Grid container spacing={2}>
						<Grid item xs={12}>
							<ContentBox name="Hero info" />
						</Grid>
						<Grid item xs={12}>
							<ContentBox name="Setting hero auto" />
						</Grid>
					</Grid>
				</Grid>
				<Grid item xs={4}>
					<Grid container spacing={2}>
						<Grid item xs={12}>
							<ContentBox name="adventures list" />
						</Grid>
						<Grid item xs={12}>
							<ContentBox name="Currently equip" />
						</Grid>
						<Grid item xs={12}>
							<ContentBox name="Update hero info time" />
						</Grid>
					</Grid>
				</Grid>
				<Grid item xs={4}>
					<Grid container spacing={2}>
						<Grid item xs={12}>
							<ContentBox name="Hero items" />
						</Grid>
					</Grid>
				</Grid>
			</Grid>
		</ContentBox>
	);
};

export default Hero;
